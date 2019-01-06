using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Network.Observation;

namespace TheRuleOfSilvester.Network
{
    public abstract class BaseClient : IObservable<Package>
    {
        public event EventHandler<(byte[] Data, int Length)> OnMessageReceived;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;

        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        private readonly HashSet<IObserver<Package>> observers;
        private readonly SemaphoreSlim semaphoreSlim;

        protected BaseClient(Socket socket)
        {

            semaphoreSlim = new SemaphoreSlim(1, 1);
            observers = new HashSet<IObserver<Package>>();

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

        public Task Start()
        {
            return Task.Run(() =>
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            });
        }

        public void Disconnect()
        {
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

        public byte[] Send(byte[] data)
        {
            var resetEvent = new ManualResetEvent(false);
            (byte[] Data, int Length) localData = (new byte[0], 0);

            void messageReceived(object sender, (byte[] Data, int Length) args)
            {
                localData = args;
                resetEvent.Set();
            }

            OnMessageReceived += messageReceived;
            Send(data, data.Length);
            resetEvent.WaitOne();
            OnMessageReceived -= messageReceived;
            return localData.Data.Take(localData.Length).ToArray();
        }

        protected virtual int ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount, int offset)
        {
            if (receiveArgsCount < 2)
                receiveArgsCount = 2; //because of the substraction in the next lines

            (short Command, byte[] Data) = (0, new byte[receiveArgsCount - 2]);
            Command = BitConverter.ToInt16(receiveArgsBuffer, 0);
            Array.Copy(receiveArgsBuffer, 2, Data, 0, receiveArgsCount - 2);

            semaphoreSlim.Wait();

            foreach (var observer in observers)
                observer.OnNext(new Package(Command, Data)
                {
                    Client = this
                });

            semaphoreSlim.Release();

            OnMessageReceived?.Invoke(this,(Data, Data.Length));
            return receiveArgsCount;
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
            Receive(e);
        }

        protected void Receive(SocketAsyncEventArgs e)
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

            } while (!Socket.ReceiveAsync(e));
        }

        public IDisposable Subscribe(IObserver<Package> observer)
        {
            semaphoreSlim.Wait();
            observers.Add(observer);
            semaphoreSlim.Release();
            return new Subscription(observer, this);
        }
    }
}
