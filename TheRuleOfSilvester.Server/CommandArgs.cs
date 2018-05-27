using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Server
{
    public class CommandArgs
    {
        public NetworkPlayer NetworkPlayer { get; set; }
        public byte[] Data { get; set; }
        public bool HavePlayer => NetworkPlayer != null;

        public CommandArgs(NetworkPlayer player, byte[] data)
        {
            NetworkPlayer = player;
            Data = data;
        }
    }
}
