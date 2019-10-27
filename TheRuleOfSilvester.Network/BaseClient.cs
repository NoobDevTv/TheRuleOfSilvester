using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Network
{
    public abstract class BaseClient : INotificationObservable<Package>
    {
        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;

        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        private readonly HashSet<INotificationObserver<Package>> observers;
        private readonly SemaphoreExtended semaphoreSlim;

        protected BaseClient(Socket socket)
        {

            semaphoreSlim = new SemaphoreExtended(1, 1);
            observers = new HashSet<INotificationObserver<Package>>();

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

        public void Send(Package package)
        {
            Send(package.ToByteArray(), package.Data.Length + Package.HEADER_SIZE);
        }

        protected virtual int ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount, int offset)
        {
            var data = new byte[receiveArgsCount];
            Buffer.BlockCopy(receiveArgsBuffer, offset, data, 0, receiveArgsCount);
            var package = Package.FromByteArray(data);
            package.Client = this;

            CallOnNext(package);

            return receiveArgsCount;
        }

        protected virtual void CallOnNext(Package package)
        {
            using (semaphoreSlim.Wait())
            {
                foreach (var observer in observers)
                    observer.OnNext(package);
            }
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

        public IDisposable Subscribe(INotificationObserver<Package> observer)
        {
            using (semaphoreSlim.Wait())
                observers.Add(observer);

            return new Subscription<Package>(observer, this);
        }

        public void OnDispose(INotificationObserver<Package> observer)
        {
            observers.Remove(observer);
        }
    }
}
