using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommands
    {
        [Command((short)CommandNames.TransmitActions)]
        public static byte[] TransmitActions(byte[] data)
        {
            int playerID = BitConverter.ToInt32(data, 0);
            var playerActions = SerializeHelper.Deserialize<PlayerAction, List<PlayerAction>>(data.Skip(4).ToArray());
            GameManager.AddRoundActions(playerID, playerActions);

            return BitConverter.GetBytes((short)CommandNames.TransmitActions);
        }

        [Command((short)CommandNames.EndRound)]
        public static byte[] EndRound(byte[] data)
        {
            var playerId = BitConverter.ToInt32(data, 0);
            GameManager.EndRound(playerId);
            return BitConverter.GetBytes((short)CommandNames.EndRound);
        }
    }
}
