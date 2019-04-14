using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Items;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    internal class PlanningRoundComponent : IRoundComponent
    {
        public uint NextOrder => currentOrder++;

        private uint currentOrder;

        public RoundMode Round => RoundMode.Planning;

        public bool RoundEnd { get; set; }

        private Player player;

        private Stack<PlayerAction> actions;

        private bool propertyChangedRelevant = true;

        public PlanningRoundComponent()
        {
            currentOrder = 1;
        }

        public void Update(Game game)
        {
            //Undo Button iplementation
            if (game.InputAction == null)
                return;

            if (game.InputAction.Type == InputActionType.RoundActionButton && game.InputAction.Valid)
            {
                propertyChangedRelevant = false;
                UndoLastMovement(game.Map);
                propertyChangedRelevant = true;
            }
        }

        public void Start(Game game)
        {
            game.InputCompoment.Active = true;
            player = game.Map.Players.FirstOrDefault(x => x.IsLocal);
            actions = new Stack<PlayerAction>(player.Role.ActionsPoints);
            Subscribe();
        }

        public void Stop(Game game)
        {
            Desubscribe();

            game.MultiplayerComponent?.TransmitActions(actions, player);
            currentOrder = 1;
            int z = actions.Count;
            for (int i = 0; i < z; i++)
                UndoLastMovement(game.Map);

            game.MultiplayerComponent?.EndRound();
        }

        public void UndoLastMovement(Map map)
        {
            if (actions.Count == 0)
                return;

            var move = actions.Pop();
            player.Role.SetUsedActionPoints(actions.Count);
            switch (move.ActionType)
            {
                case ActionType.Moved:
                    player.MoveGeneral(new Point(player.Position.X - move.Point.X, player.Position.Y - move.Point.Y));
                    map
                        .Cells
                        .Where(c => typeof(BaseItemCell).IsAssignableFrom(c.GetType()))
                        .ToList()
                        .ForEach(i => i.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:

                    var inventoryCell = player.CellInventory.Last();
                    player.CellInventory.Remove(inventoryCell);

                    var mapCell = map.SwapInventoryAndMapCell(inventoryCell, move.Point, 1) as MapCell;

                    //TODO Reduce duplicated code
                    player.CellInventory.ForEach(x => { x.Position = new Point(x.Position.X + 2, x.Position.Y); x.Invalid = true; });
                    player.CellInventory.Add(mapCell, 0);
                    break;
                case ActionType.CollectedItem:
                    var item = player.ItemInventory.LastOrDefault();
                    item.Position = player.Position;
                    map.Cells.Add(item);
                    player.ItemInventory.Remove(item);
                    item.Invalid = true;
                    player.Role.RedrawStats = true;
                    break;
            }
        }

        private void Subscribe()
        {
            player.PropertyChange += PlayerPropertyChange;
            player.PropertyChanged += PlayerPropertyChanged;

            player.PlayerChangedCell += OnPlayerChangedCell;
        }

        private void Desubscribe()
        {
            player.PropertyChange -= PlayerPropertyChange;
            player.PropertyChanged -= PlayerPropertyChanged;

            player.PlayerChangedCell -= OnPlayerChangedCell;
        }

        private void OnPlayerChangedCell(object sender, Cell e)
        {
            PlayerAction action;

            if (sender is Player senderPlayer)
                action = new PlayerAction(senderPlayer, ActionType.ChangedMapCell, e.Position);
            else
                action = new PlayerAction(player, ActionType.ChangedMapCell, e.Position);

            action.Order = NextOrder;
            actions.Push(action);
            player.Role.SetUsedActionPoints(actions.Count);
        }

        private void PlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyChangedRelevant = false;
            //Temp maybe make it different
            while (player.Role.RestActionPoints < 0)
                UndoLastMovement(player.Map);

            propertyChangedRelevant = true;

            if (e.PropertyName == nameof(Player.Position) && player.TryCollectItem())
            {
                PlayerAction action;
                if (sender is Player senderPlayer)
                    action = new PlayerAction(senderPlayer, ActionType.CollectedItem, senderPlayer.Position);
                else
                    action = new PlayerAction(player, ActionType.CollectedItem, player.Position);

                action.Order = NextOrder;
                actions.Push(action);
                player.Role.SetUsedActionPoints(actions.Count);
            }
        }

        private void PlayerPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (e.PropertyName != nameof(Player.Position))
                return;

            var newPos = (Point)e.NewValue;
            var oldPos = (Point)e.OldValue;

            if (propertyChangedRelevant)
            {
                PlayerAction action;

                if (sender is Player senderPlayer)
                    action = new PlayerAction(senderPlayer, ActionType.Moved, new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y));
                else
                    action = new PlayerAction(player, ActionType.Moved, new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y));

                action.Order = NextOrder;
                actions.Push(action);
                player.Role.SetUsedActionPoints(actions.Count);
            }
        }
    }
}
