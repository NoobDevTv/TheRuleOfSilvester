using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime.RoundComponents
{
    internal class ExecutingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Executing;

        public bool RoundEnd { get; set; }
        public Queue<PlayerAction> CurrentUpdateSets { get; private set; }

        private int updateCount;

        public void Start(Game game)
        {
            CurrentUpdateSets = new Queue<PlayerAction>(game.CurrentUpdateSets);
        }

        public void Stop(Game game)
        {
            game.MultiplayerComponent?.EndRound();
            if (game.MultiplayerComponent.CurrentServerStatus == Network.ServerStatus.Ended)
            {
                game.Winners = game.MultiplayerComponent.GetWinners().ToList();                
                game.Stop();
            }
        }

        public void Update(Game game)
        {
            updateCount++;

            if (game.Frames / 2 != updateCount)
                return;

            if (CurrentUpdateSets.Count < 1)
                return;

            PlayerAction action = CurrentUpdateSets.Dequeue();

            var localUpdatePlayer = game.Map.Players.First(p => p == action.Player as PlayerCell);

            switch (action.ActionType)
            {
                case ActionType.Moved:
                    localUpdatePlayer.MoveGeneralRelative(action.Point);
                    game.Map.Players.ForEach(x => x.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:
                    var inventoryCell = localUpdatePlayer.CellInventory.First(x => x.Position.X == 1);
                    localUpdatePlayer.CellInventory.Remove(inventoryCell);

                    var mapCell = game.Map.SwapInventoryAndMapCell(inventoryCell, action.Point) as MapCell;

                    localUpdatePlayer.CellInventory.ForEach(x => { x.Position = new Position(x.Position.X - 2, x.Position.Y); x.Invalid = true; });
                    localUpdatePlayer.CellInventory.Add(mapCell);
                    localUpdatePlayer.Invalid = true;
                    break;
                case ActionType.CollectedItem:
                    localUpdatePlayer.TryCollectItem();
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
