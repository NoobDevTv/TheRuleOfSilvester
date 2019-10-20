using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class RoundCommandObserver : CommandObserver
    {
        private readonly GameManager gameManager;
        private readonly PlayerService playerService;

        public RoundCommandObserver(GameManager gameManager, PlayerService playerService)
        {
            this.gameManager = gameManager;
            this.playerService = playerService;
        }

        public override object OnNext(CommandNotification value) => value.CommandName switch
        {
            CommandName.TransmitActions => TransmitActions(value.Arguments),
            CommandName.EndRound => EndRound(value.Arguments),
            CommandName.Wait => Wait(value.Arguments),

            _ => default,
        };

        public short TransmitActions(CommandArgs args)
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var networkPlayer))
                throw new NotSupportedException();

            var playerActions = SerializeHelper.DeserializeToList<PlayerAction>(args.Data.ToArray()).ToList();
            gameManager.AddRoundActions(networkPlayer.Player, playerActions.OrderBy(a => a.Order).ToList());

            return (short)CommandName.TransmitActions;
        }

        public short EndRound(CommandArgs args)
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var networkPlayer))
                throw new NotSupportedException();

            gameManager.EndRound(networkPlayer);
            return (short)CommandName.EndRound;
        }

        public byte[] Wait(CommandArgs args)
        {
            if (!playerService.TryGetNetworkPlayer(args.Client.Player, out var networkPlayer))
                throw new NotSupportedException();

            return BitConverter
                 .GetBytes(networkPlayer.RoundMode == RoundMode.Executing)
                 .Concat(SerializeHelper.SerializeList(networkPlayer.UpdateSets ?? new List<PlayerAction>()))
                 .ToArray();
        }
    }
}
