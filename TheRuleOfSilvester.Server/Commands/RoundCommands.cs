using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommands
    {
        [Command(CommandNames.TransmitMoves)]
        public static byte[] TransmitMoves(byte[] data)
        {
            //TODO Player + Action auf Server, um die Daten zu interpretieren

            using (var memoryStream = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                binaryReader.ReadInt32(); //TODO Haben wir eine PlayerID?
                while (memoryStream.Position != memoryStream.Length)
                {
                    var move = new PlayerAction();
                    move.Deserialize(binaryReader);
                }
            }
        }
    }
}
