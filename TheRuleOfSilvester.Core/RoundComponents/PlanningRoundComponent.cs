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

        public bool RoundEnd { get; private set; }

        private readonly int maxMoves = 70;

        private Player player;

        private Stack<PlayerAction> actions;

        private bool propertyChangedRelevant = true;

        public void Update(Game game)
        {
            //Undo Button iplementation
            if (game.InputCompoment.RoundActionButton)
            {
                propertyChangedRelevant = false;
                UndoLastMovement(game);
                propertyChangedRelevant = true;
            }
            if (game.InputCompoment.StartAction)
            {
            }
        }

      

        public void Start(Game game)
        {
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
            
            game.MultiplayerComponent?.EndRound(game.Map.Players.First(p => p.IsLocal));
        }

        public void UndoLastMovement(Game game = null)
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

                    var inventoryCell = player.Inventory.Last();
                    var mapCell = game.Map.Cells.First(x => x.Position == move.Point);

                    player.Inventory.Remove(inventoryCell);
                    game.Map.Cells.Remove(mapCell);

                    inventoryCell.Position = move.Point;
                    game.Map.Cells.Add(inventoryCell);


                    mapCell.Position = new Point(1, game.Map.Height + 2);
                    inventoryCell.Invalid = true;
                    mapCell.Invalid = true;

                    //TODO Reduce duplicated code
                    player.Inventory.ForEach(x => { x.Position = new Point(x.Position.X + 2, x.Position.Y); x.Invalid = true; });
                    player.Inventory.Insert(0, mapCell);

                    var cellsToNormalize = game.Map.Cells.Where(c =>
                                          c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y - 1
                                      || c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y + 1
                                      || c.Position.X == inventoryCell.Position.X - 1 && c.Position.Y == inventoryCell.Position.Y
                                      || c.Position.X == inventoryCell.Position.X + 1 && c.Position.Y == inventoryCell.Position.Y)
                                      .Select(x => (MapCell)x).ToList();
                    cellsToNormalize.ForEach(x => x.NormalizeLayering());

                    (inventoryCell as MapCell).NormalizeLayering();
                    (mapCell as MapCell).NormalizeLayering();

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
                UndoLastMovement();

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
