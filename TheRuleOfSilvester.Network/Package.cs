using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public class Package
    {
        
        public short Command { get; set; }
        public byte[] Data { get; set; }
        public CommandName CommandName => (CommandName)Command;

        public BaseClient Client { get; set; }

        public Package(short command, byte[] data)
        {
            Command = command;
            Data = data;
        }

        public override string ToString() => $"{CommandName} [{Data.Length}]";

    }
}
