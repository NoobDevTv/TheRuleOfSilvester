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

        private int maxMoves = 70;

        private Player player;

        private Stack<PlayerAction> moves;

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

        private void Player_PropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (e.PropertyName != "Position")
                return;

            Point newPos = (Point)e.NewValue;
            Point oldPos = (Point)e.OldValue;

            if (propertyChangedRelevant)
                moves.Push(new PlayerAction(ActionType.Moved, new Point(newPos.X - oldPos.X, newPos.Y - oldPos.Y)));

        }

        public void Start(Game game)
        {
            player = game.Map.Players.FirstOrDefault(x => x.IsLocal);
            moves = new Stack<PlayerAction>(maxMoves);
            Subscribe();
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyChangedRelevant = false;
            //Temp maybe make it different
            while (moves.Count > maxMoves)
                UndoLastMovement();

            propertyChangedRelevant = true;
        }

        public void Stop(Game game)
        {
            Desubscive();
            //TODO Transmit moves

        }

        private void Subscribe()
        {
            player.PropertyChange += Player_PropertyChange;
            player.PropertyChanged += Player_PropertyChanged;
        }
        private void Desubscive()
        {
            player.PropertyChange -= Player_PropertyChange;
            player.PropertyChanged -= Player_PropertyChanged;
        }

        public void UndoLastMovement(Game game = null)
        {
            if (moves.Count == 0)
                return;

            var move = moves.Pop();
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
