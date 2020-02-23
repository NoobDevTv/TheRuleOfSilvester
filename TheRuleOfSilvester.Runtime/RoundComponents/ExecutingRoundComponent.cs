using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Runtime.Cells;

namespace TheRuleOfSilvester.Runtime.RoundComponents
{
    internal class ExecutingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Executing;

        public bool RoundEnd { get; set; }

        private int updateCount;
        private Game game;
        private readonly SerialDisposable disposables;
        private readonly Subject<bool> endRound;

        public ExecutingRoundComponent()
        {
            disposables = new SerialDisposable();
            endRound = new Subject<bool>();
        }

        public void Start(Game game)
        {
            this.game = game;

            var currentUpdateSet = Observable.Zip(game.CurrentUpdateSets, Observable.Interval(TimeSpan.FromMilliseconds(250)), (update, interval) => update);

            var compositeDisposable = new CompositeDisposable {
                 currentUpdateSet
                            .Subscribe(ExecuteAction, e => { }, () => RoundEnd = true)
            };

            if (game.IsMutliplayer)
            {
                compositeDisposable.Add(game.MultiplayerComponent
                        .CurrentServerStatus
                        .Where(s => s == ServerStatus.Ended)
                        .Subscribe(s =>
                        {
                            game.Stop();
                        }));

                compositeDisposable.Add(game.MultiplayerComponent
                        .SendPackages(endRound.Select(v => (CommandName.EndRound, Notification.Empty))));
            }

            disposables.Disposable = compositeDisposable;
        }

        private void ExecuteAction(PlayerAction action)
        {
            PlayerCell localUpdatePlayer = game.Map.Players.First(p => p == action.Player as PlayerCell);

            switch (action.ActionType)
            {
                case ActionType.Moved:
                    localUpdatePlayer.MoveGeneralRelative(action.Point);
                    game.Map.Players.ForEach(x => x.Invalid = true);
                    break;
                case ActionType.ChangedMapCell:
                    MapCell inventoryCell = localUpdatePlayer.CellInventory.First(x => x.Position.X == 1);
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
        }

        public void Stop(Game game)
        {
            endRound.OnNext(true);
            disposables.Disposable = Disposable.Empty;
        }

        public void Update(Game game)
        {
            updateCount++;

            if (game.Frames / 2 != updateCount)
                return;
        }
    }
}
