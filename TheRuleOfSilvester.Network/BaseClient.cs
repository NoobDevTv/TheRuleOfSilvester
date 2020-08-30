using NLog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Network
{
    public abstract class BaseClient : IDisposable
    {
        public IObservable<Package> ReceivedPackages => packageSubject;

        public event EventHandler OnDisconnected;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;

        private Task internalTask;
        private CancellationTokenSource tokenSource;

        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        private readonly Subject<Package> packageSubject;
        private readonly Logger logger;

        protected BaseClient(Socket socket)
        {
            packageSubject = new Subject<Package>();
            logger = LogManager.GetCurrentClassLogger();

            sendQueue = new (byte[] data, int len)[256];
            sendLock = new object();

            Socket = socket;
            Socket.NoDelay = true;
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(20480), 0, 20480);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

        }

        public void Start()
        {
            tokenSource = new CancellationTokenSource();
            internalTask = Task.Run(() =>
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs, tokenSource.Token);
            }, tokenSource.Token);
        }

        public void Stop()
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }

        public void Disconnect()
        {
            Send(new Package(CommandName.Disconnect, Array.Empty<byte>()));
            Socket.Disconnect(false);
        }

        public void Send(byte[] data, int len)
        {
            lock (sendLock)
            {
                if (sending)
                {
                    sendQueue[nextSendQueueWriteIndex++] = (data, len);
                    return;
                }

                sending = true;
            }

            SendInternal(data, len);

        }

        public void Send(Package package)
        {
            logger.Debug($"Send: [{package.Id}] {package.CommandName} ({package.Data?.Length})");
            Send(package.ToByteArray(), package.Data.Length + Package.HEADER_SIZE);
        }


        protected virtual int ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount, int offset)
        {
            var data = new byte[receiveArgsCount];
            Buffer.BlockCopy(receiveArgsBuffer, offset, data, 0, receiveArgsCount);
            var package = Package.FromByteArray(data);
            package.Client = this;

            if (package.CommandName == CommandName.Disconnect)
            {
                Socket.Close();
                packageSubject.OnCompleted();
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
            else
            {
               CallOnNext(package);
            }

            return package.Data.Length + Package.HEADER_SIZE;
        }

        protected virtual void CallOnNext(Package package)
        {
            logger.Debug($"Received: [{package.Id}] {package.CommandName} ({package.Data?.Length})");
            packageSubject.OnNext(package);
        }

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(sendArgs))
                    return;

                lock (sendLock)
                {
                    if (readSendQueueIndex < nextSendQueueWriteIndex)
                    {
                        (data, len) = sendQueue[readSendQueueIndex++];
                    }
                    else
                    {
                        nextSendQueueWriteIndex = 0;
                        readSendQueueIndex = 0;
                        sending = false;
                        return;
                    }
                }
            }
        }

        private void OnSent(object sender, SocketAsyncEventArgs e)
        {
            byte[] data;
            int len;

            ArrayPool<byte>.Shared.Return(e.Buffer);

            lock (sendLock)
            {
                if (readSendQueueIndex < nextSendQueueWriteIndex)
                {
                    (data, len) = sendQueue[readSendQueueIndex++];
                }
                else
                {
                    nextSendQueueWriteIndex = 0;
                    readSendQueueIndex = 0;
                    sending = false;
                    return;
                }
            }

            SendInternal(data, len);
        }

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            var token = tokenSource?.Token ?? CancellationToken.None;
            Receive(e, token);
        }

        protected void Receive(SocketAsyncEventArgs e, CancellationToken token)
        {
            do
            {
                if (e.BytesTransferred < 1)
                    return;

                int offset = 0;

                do
                {
                    offset += ProcessInternal(e.Buffer, e.BytesTransferred, offset);

                } while (offset < e.BytesTransferred);

            } while (!token.IsCancellationRequested && !Socket.ReceiveAsync(e));
        }


        public IDisposable SendPackages(IObservable<Package> packages)
            => packages.Subscribe(Send);


        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                tokenSource?.Dispose();

                Socket.Dispose();
                sendArgs.Dispose();
                ReceiveArgs.Dispose();
                packageSubject.Dispose();

                OnDisconnected = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~BaseClient()
        {
            Dispose(false);
        }
    }
}
