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
        public event EventHandler<(short Command, byte[] Data)> OnCommandReceived;

        public ConnectedClient(Socket socket) : base(socket)
        {

        }

        protected override void ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount)
        {
            (short Command, byte[] Data) = (0, new byte[receiveArgsCount - 2]);
            Command = (short)(receiveArgsBuffer[0] << 8 | receiveArgsBuffer[1]);
            Array.Copy(receiveArgsBuffer, 2, Data, 0, receiveArgsCount - 2);
            OnCommandReceived?.Invoke(this, (Command, Data));
        }

    }
}
