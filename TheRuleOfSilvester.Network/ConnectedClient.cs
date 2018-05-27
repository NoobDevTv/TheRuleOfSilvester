using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheRuleOfSilvester.Network
{
    public class ConnectedClient : BaseClient
    {
        private static int received;

        public int PlayerId { get; set; }

        public bool Registered => PlayerId > 0;

        public event EventHandler<(short Command, byte[] Data)> OnCommandReceived;

        public ConnectedClient(Socket socket) : base(socket)
        {
            PlayerId = -1;
        }

        protected override void ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount)
        {
            (short Command, byte[] Data) = (0, new byte[receiveArgsCount - 2]);
            Command = BitConverter.ToInt16(receiveArgsBuffer, 0);
            Array.Copy(receiveArgsBuffer, 2, Data, 0, receiveArgsCount - 2);
            OnCommandReceived?.Invoke(this, (Command, Data));
        }

    }
}
