using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Items;

namespace TheRuleOfSilvester.Runtime.RoundComponents
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
        private readonly Subject<IEnumerable<PlayerAction>> playerActions;
        private readonly SerialDisposable disposables;
        private readonly Subject<bool> endRound;

        public PlanningRoundComponent()
        {
            currentOrder = 1;
            playerActions = new Subject<IEnumerable<PlayerAction>>();
            endRound = new Subject<bool>();
            disposables = new SerialDisposable();
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
            player = game.Map.Players.OfType<Player>().FirstOrDefault(x => x.IsLocal);
            actions = new Stack<PlayerAction>(player.Role.ActionsPoints);

            if (game.IsMutliplayer)
            {
                disposables.Disposable = new CompositeDisposable
                {
                   game.MultiplayerComponent.SendPackages(
                        playerActions
                            .Select(SerializeHelper.SerializeList)
                            .Select(b => (CommandName.TransmitActions, new Notification(b, NotificationType.PlayerActions)))),

                    game.MultiplayerComponent
                        .SendPackages(endRound.Select(v => (CommandName.EndRound, Notification.Empty)))
                };
            }
            else
            {
                disposables.Disposable = new CompositeDisposable
                {
                   playerActions.Subscribe(p => game.CurrentUpdateSets = p.ToArray().Reverse().ToObservable())
                };
            }

            Subscribe();
        }

        public void Stop(Game game)
        {
            Desubscribe();

            playerActions.OnNext(actions);

            currentOrder = 1;
            var z = actions.Count;
            for (var i = 0; i < z; i++)
                UndoLastMovement(game.Map);

            endRound.OnNext(true);
            disposables.Disposable = Disposable.Empty;
        }

        public void UndoLastMovement(Map map)
        {
            if (actions.Count == 0)
                return;

            PlayerAction move = actions.Pop();
            player.Role.SetUsedActionPoints(actions.Count);
            switch (move.ActionType)
            {
                case ActionType.Moved:
                    player.MoveGeneral(new Position(player.Position.X - move.Point.X, player.Position.Y - move.Point.Y));
                    map
                        .Cells
                        .Where(c => typeof(BaseItemCell).IsAssignableFrom(c.GetType()))
                        .ToList()
                        .ForEach(i => i.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:

                    MapCell inventoryCell = player.CellInventory.Last();
                    player.CellInventory.Remove(inventoryCell);

                    var mapCell = map.SwapInventoryAndMapCell(inventoryCell, move.Point, 1) as MapCell;

                    //TODO Reduce duplicated code
                    player.CellInventory.ForEach(x => { x.Position = new Position(x.Position.X + 2, x.Position.Y); x.Invalid = true; });
                    player.CellInventory.Add(mapCell, 0);
                    break;
                case ActionType.CollectedItem:
                    BaseItemCell item = player.ItemInventory.Last();
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

            var newPos = (Position)e.NewValue;
            var oldPos = (Position)e.OldValue;

            if (propertyChangedRelevant)
            {
                PlayerAction action;

                if (sender is Player senderPlayer)
                    action = new PlayerAction(senderPlayer, ActionType.Moved, new Position(newPos.X - oldPos.X, newPos.Y - oldPos.Y));
                else
                    action = new PlayerAction(player, ActionType.Moved, new Position(newPos.X - oldPos.X, newPos.Y - oldPos.Y));

                action.Order = NextOrder;
                actions.Push(action);
                player.Role.SetUsedActionPoints(actions.Count);
            }
        }
    }
}
