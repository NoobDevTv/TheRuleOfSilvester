using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public abstract class BaseClient
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

        protected BaseClient(Socket socket)
        {
            sendQueue = new(byte[] data, int len)[256];
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
            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                ProcessInternal(ReceiveArgs.Buffer, ReceiveArgs.BytesTransferred);
            }
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

        protected abstract void ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount);

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(sendArgs))
                    return;

                //ArrayPool<byte>.Shared.Return(data);

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
            ProcessInternal(e.Buffer, e.BytesTransferred);

            OnMessageReceived?.Invoke(this, (e.Buffer, e.BytesTransferred));

            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                ProcessInternal(ReceiveArgs.Buffer, ReceiveArgs.BytesTransferred);
            }
        }
    }
}
