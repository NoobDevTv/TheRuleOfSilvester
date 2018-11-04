using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    class PlanningRoundComponent : IRoundComponent
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
                    break;
                case ActionType.ChangedMapCell:

                    var inventoryCell = player.CellInventory.Last();
                    player.CellInventory.Remove(inventoryCell);

                    var mapCell = game.Map.SwapInventoryAndMapCell(inventoryCell, move.Point, 1);

                    //TODO Reduce duplicated code
                    player.CellInventory.ForEach(x => { x.Position = new Point(x.Position.X + 2, x.Position.Y); x.Invalid = true; });
                    player.CellInventory.Insert(0, mapCell);
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
            actions.Push(new PlayerAction(ActionType.ChangedMapCell, e.Position));
        }

        private void PlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyChangedRelevant = false;
            //Temp maybe make it different
            while (actions.Count > maxMoves)
                UndoLastMovement(null);

            propertyChangedRelevant = true;
        }

        private void PlayerPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (e.PropertyName != "Position")
                return;

            Point newPos = (Point)e.NewValue;
            Point oldPos = (Point)e.OldValue;

            if (propertyChangedRelevant)
                actions.Push(new PlayerAction(ActionType.Moved, new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y)));

        }
    }
}
