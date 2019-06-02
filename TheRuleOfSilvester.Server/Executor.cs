using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Server
{
    internal class Executor
    {
        public List<PlayerAction> UpdateSets { get; }

        private readonly ActionCache actionCache;

        public Executor(ActionCache actionCache)
        {
            this.actionCache = actionCache;
            UpdateSets = new List<PlayerAction>();
        }

        public void Execute(Map map)
        {
            UpdateSets.Clear();

            foreach (var actions in actionCache)
            {
                foreach (var action in actions)
                {
                    var player = action.Player;

                    switch (action.ActionType)
                    {
                        case ActionType.Moved:
                            var point = new Position(player.Position.X + action.Point.X, player.Position.Y + action.Point.Y);
                            map.Players.First(p => p == player).MoveGeneral(point);
                            break;
                        case ActionType.ChangedMapCell:
                            var cell = map.Cells.First(c => c.Position == action.Point);
                            map.Cells.Remove(cell);
                            var inventoryCell = player.CellInventory.First();
                            inventoryCell.Position = cell.Position;
                            inventoryCell.Invalid = true;
                            map.Cells.Add(inventoryCell);
                            player.CellInventory.Remove(inventoryCell);
                            player.CellInventory.Add(cell as MapCell);

                            cell.Position = new Position(5, map.Height + 2);
                            cell.Invalid = true;
                            player.CellInventory.ForEach(x =>
                            {
                                x.Position = new Position(x.Position.X - 2, x.Position.Y);
                                x.Invalid = true;
                            });
                            break;
                        case ActionType.CollectedItem:
                            player.TryCollectItem();
                            break;
                        case ActionType.None:
                        default:
                            break;

                    }

                    UpdateSets.Add(action);
                }
            }
        }
    }
}