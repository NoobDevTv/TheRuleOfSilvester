using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    internal class ExecutingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Executing;

        public bool RoundEnd { get; set; }
        public Stack<(Player Player, PlayerAction Action)> CurrentUpdateSets { get; private set; }

        private int updateCount;

        public void Start(Game game)
        {
            CurrentUpdateSets = new Stack<(Player Player, PlayerAction Action)>(game.CurrentUpdateSets
                .SelectMany(s => s.PlayerActions.Select(p => (s.Player, p))));
        }

        public void Stop(Game game) => game.MultiplayerComponent?.EndRound();

        public void Update(Game game)
        {
            updateCount++;

            if (game.Frames / 2 != updateCount)
                return;

            if (CurrentUpdateSets.Count < 1)
                return;

            (Player player, PlayerAction action) = CurrentUpdateSets.Pop();

            var localUpdatePlayer = game.Map.Players.First(p => p == player);

            switch (action.ActionType)
            {
                case ActionType.Moved:
                    localUpdatePlayer.MoveGeneralRelative(action.Point);
                    game.Map.Players./*Where(p => p.Position == action.Point).ToList().*/ForEach(x => x.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:
                    var inventoryCell = localUpdatePlayer.CellInventory.First(x => x.Position.X == 1);
                    localUpdatePlayer.CellInventory.Remove(inventoryCell);

                    var mapCell = game.Map.SwapInventoryAndMapCell(inventoryCell, action.Point);

                    localUpdatePlayer.CellInventory.ForEach(x => { x.Position = new Point(x.Position.X - 2, x.Position.Y); x.Invalid = true; });
                    localUpdatePlayer.CellInventory.Insert(0, mapCell);
                    localUpdatePlayer.Invalid = true;
                    break;
                case ActionType.None:
                default:
                    break;
            }

            updateCount = 0;

            if (CurrentUpdateSets.Count == 0)
                RoundEnd = true;
        }
    }
}
