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
        public RoundMode Round => RoundMode.Planning;

        public bool RoundEnd { get; set; }

        private readonly int maxMoves = 70;

        private Player player;

        private Stack<PlayerAction> actions;

        private bool propertyChangedRelevant = true;

        public void Update(Game game)
        {
            //Undo Button iplementation
            if (game.InputAction == null)
                return;

            if (game.InputAction.Type == InputActionType.RoundActionButton && game.InputAction.Valid)
            {
                propertyChangedRelevant = false;
                UndoLastMovement(game);
                propertyChangedRelevant = true;
            }
        }

        public void Start(Game game)
        {
            game.InputCompoment.Active = true;
            player = game.Map.Players.FirstOrDefault(x => x.IsLocal);
            actions = new Stack<PlayerAction>(maxMoves);
            Subscribe();
        }

        public void Stop(Game game)
        {
            Desubscribe();

            game.MultiplayerComponent?.TransmitActions(actions, player);

            int z = actions.Count;
            for (int i = 0; i < z; i++)
                UndoLastMovement(game);

            game.MultiplayerComponent?.EndRound();
        }

        public void UndoLastMovement(Game game)
        {
            if (actions.Count == 0)
                return;

            var move = actions.Pop();
            switch (move.ActionType)
            {
                case ActionType.Moved:
                    player.MoveGeneral(new Point(player.Position.X - move.Point.X, player.Position.Y - move.Point.Y));
                    game.Map
                        .Cells
                        .Where(c => typeof(BaseItemCell).IsAssignableFrom(c.GetType()))
                        .ToList()
                        .ForEach(i => i.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:

                    var inventoryCell = player.CellInventory.Last();
                    player.CellInventory.Remove(inventoryCell);

                    var mapCell = game.Map.SwapInventoryAndMapCell(inventoryCell, move.Point, 1);

                    //TODO Reduce duplicated code
                    player.CellInventory.ForEach(x => { x.Position = new Point(x.Position.X + 2, x.Position.Y); x.Invalid = true; });
                    player.CellInventory.Insert(0, mapCell);
                    break;
                case ActionType.CollectedItem:
                    var itemIndex = Array.FindLastIndex(player.ItemInventory, x => x != null);
                    var item = player.ItemInventory[itemIndex];
                    item.Position = player.Position;
                    game.Map.Cells.Add(item);
                    player.ItemInventory[itemIndex] = null;
                    item.Invalid = true;
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

            actions.Push(action);
        }

        private void PlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyChangedRelevant = false;
            //Temp maybe make it different
            while (actions.Count > maxMoves)
                UndoLastMovement(null);

            propertyChangedRelevant = true;

            if (player.TryCollectItem() && e.PropertyName == nameof(Player.Position))
            {
                PlayerAction action;
                if (sender is Player senderPlayer)
                    action = new PlayerAction(senderPlayer, ActionType.CollectedItem, senderPlayer.Position);
                else
                    action = new PlayerAction(player, ActionType.CollectedItem, player.Position);

                actions.Push(action);
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

                actions.Push(action);
            }
        }
    }
}
