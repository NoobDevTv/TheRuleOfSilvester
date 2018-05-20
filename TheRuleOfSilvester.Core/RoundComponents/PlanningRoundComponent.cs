using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Drawing;

namespace TheRuleOfSilvester.Core.RoundComponents
{
    class PlanningRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Planning;

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

        private void PlayerPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (e.PropertyName != "Position")
                return;

            Point newPos = (Point)e.NewValue;
            Point oldPos = (Point)e.OldValue;

            if (propertyChangedRelevant)
                actions.Push(new PlayerAction(ActionType.Moved, new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y)));

        }

        public void Start(Game game)
        {
            player = game.Map.Players.FirstOrDefault(x => x.IsLocal);
            actions = new Stack<PlayerAction>(maxMoves);
            Subscribe();
        }

        private void PlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyChangedRelevant = false;
            //Temp maybe make it different
            while (actions.Count > maxMoves)
                UndoLastMovement();

            propertyChangedRelevant = true;
        }

        public void Stop(Game game)
        {
            Desubscribe();

            game.MultiplayerComponent?.TransmitActions(actions, player);
            game.MultiplayerComponent?.EndRound(game.Map.Players.First(p => p.IsLocal));
        }

        private void Subscribe()
        {
            player.PropertyChange += PlayerPropertyChange;
            player.PropertyChanged += PlayerPropertyChanged;
        }
        private void Desubscribe()
        {
            player.PropertyChange -= PlayerPropertyChange;
            player.PropertyChanged -= PlayerPropertyChanged;
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
                    //TODO Implement undo Map Changes
                    break;
            }
        }
    }
}
