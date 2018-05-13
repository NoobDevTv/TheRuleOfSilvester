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
        [Command(CommandNames.TransmitActions)]
        public static byte[] TransmitActions(byte[] data)
        {
            var playerActions = new List<PlayerAction>();
            int playerID;

            using (var memoryStream = new MemoryStream(data))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                playerID = binaryReader.ReadInt32(); //TODO Haben wir eine PlayerID? Ja jetzt schon
                while (memoryStream.Position != memoryStream.Length)
                {
                    var action = new PlayerAction();
                    action.Deserialize(binaryReader);
                    playerActions.Add(action);
                }
            }

            GameManager.AddRoundActions(playerID, playerActions);

            return BitConverter.GetBytes((short)CommandNames.TransmitActions);
        }

        [Command(CommandNames.EndRound)]
        public static byte[] EndRound(byte[] data)
        {
            var playerId = BitConverter.ToInt32(data, 0);
            GameManager.EndRound(playerId);
            return BitConverter.GetBytes((short)CommandNames.EndRound);
        }
    }
}
