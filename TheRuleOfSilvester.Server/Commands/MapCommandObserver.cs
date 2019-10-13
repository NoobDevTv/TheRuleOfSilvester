using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public class MapCommandObserver : CommandObserver
    {
        private readonly GameManager gameManager;

        public MapCommandObserver(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.GetMap => GetMap(value.Arguments),
            CommandName.GetPlayers => GetPlayers(value.Arguments),
            CommandName.UpdatePlayer => UpdatePlayer(value.Arguments),

            _ => default,
        };

        public Map GetMap(CommandArgs args)
            => gameManager.Map;

        public IEnumerable<PlayerCell> GetPlayers(CommandArgs args)
            => gameManager.Map.Players.Where(p => p.Id != args.Client.PlayerId);

        public short UpdatePlayer(CommandArgs args)
        {
            //if(!args.HavePlayer)
            //    return BitConverter.GetBytes((short)CommandNames.UpdatePlayer);

            var newPlayer = SerializeHelper.Deserialize<Player>(args.Data);

            if (args.NetworkPlayer.Player is Cell player)
                player.Position = newPlayer.Position;
            else
                throw new NotSupportedException();

            return (short)CommandName.UpdatePlayer;
        }

    }
}
