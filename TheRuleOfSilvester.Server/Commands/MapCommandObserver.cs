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
        private readonly PlayerService playerService;

        public MapCommandObserver(GameManager gameManager, PlayerService playerService)
        {
            this.gameManager = gameManager;
            this.playerService = playerService;
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
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var networkPlayer))
                throw new NotSupportedException();
            
            return gameManager.Map.Players.Where(p => p != (PlayerCell)networkPlayer.Player);
        }

        public short UpdatePlayer(CommandArgs args)
        {
            var newPlayer = SerializeHelper.Deserialize<Player>(args.Data);

            if(!playerService.TryGetNetworkPlayer(args.Client.Player, out var networkPlayer))
                throw new NotSupportedException();

            if (networkPlayer.Player is Cell player)
                player.Position = newPlayer.Position;                

            return (short)CommandName.UpdatePlayer;
        }

    }
}
