using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Core.Roles;

namespace TheRuleOfSilvester.Core
{
    public class Game : IDisposable, IGameStatus
    {
        public ICollection<PlayerAction> CurrentUpdateSets { get; internal set; }
        public IDrawComponent DrawComponent { get; set; }
        public IInputCompoment InputCompoment { get; set; }
        public IMultiplayerComponent MultiplayerComponent { get; set; }
        public IRoundManagerComponent RoundComponent { get; set; }
        public IWaitingComponent WaitingComponent { get; set; }
        public GameStatus CurrentGameStatus { get; set; }

        public int Frames { get; private set; }

        public Map Map { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsMutliplayer { get; private set; }

        internal InputAction InputAction { get; private set; }
        public List<Player> Winners { get; internal set; }

        private int ups;
        private Thread gameThread;
        private Player player;
        private readonly ManualResetEventSlim manualResetEvent;

        public Game() 
            => manualResetEvent = new ManualResetEventSlim(false);

        public void Run(int frame, int ups, bool multiplayer, string playername = "")
        {
            IsMutliplayer = multiplayer;

            if (multiplayer) { 
                MultiplayerComponent.Connect();
                MultiplayerComponent.Wait();
            }

            Frames = frame;
            this.ups = ups;

            if (multiplayer)
            {
                Map = MultiplayerComponent.GetMap();
                player = MultiplayerComponent.ConnectPlayer(playername);
                player.Map = Map;
                player.IsLocal = true;
                player.Color = Color.Red;
            }
            else
            {
                var generator = new MapGenerator();
                Map = generator.Generate(300, 300);

                player = new Player(Map, RoleManager.GetRandomRole())
                {
                    Color = Color.Red,
                    Position = new Point(7, 4)
                };
                CurrentGameStatus = GameStatus.Running;
            }


            Map.Players.Add(player);

            if (RoundComponent == null)
                RoundComponent = new DefaultRoundManagerComponent(Map);

            IsRunning = true;

            gameThread = new Thread(Loop)
            {
                Name = "gameThread"
            };

            gameThread.Start();
        }
        public void Run(int frame, int ups) 
            => Run(frame, ups, false);

        public void Stop()
        {
            if (IsMutliplayer)
                MultiplayerComponent.Disconnect();

            if (gameThread.IsAlive)
                IsRunning = false;

            CurrentGameStatus = GameStatus.Stopped;
        }

        public void Update()
        {
            BeforeUpdate();
            SystemUpdate();
            UiUpdate();
            AfterUpdate();
        }

        public void Wait()
            => manualResetEvent.Wait();

        private void BeforeUpdate()
        {
            if (InputCompoment.InputActions.TryDequeue(out var inputAction))
            {
                InputAction = inputAction;
            }
            else
            {
                InputAction?.SetInvalid();
                InputAction = null;
            }
        }

        private void SystemUpdate()
        {
            if (IsMutliplayer)
                MultiplayerComponent.Update(this);

            WaitingComponent?.Update(this);

            if (CurrentGameStatus == GameStatus.Running)
                GameUpdate();
        }

        private void GameUpdate()
        {
            player.Update(this);
            RoundComponent.Update(this);
        }

        private void UiUpdate()
        {
            if (CurrentGameStatus == GameStatus.Running)
                DrawComponent.Draw(Map);
            else 
                DrawComponent.DrawCells(new List<TextCell> { new TextCell("NOT Running", Map) });
        }

        private void AfterUpdate()
        {
            InputAction?.SetInvalid();
            InputAction = null;
        }

        public void Dispose() =>
            //Stop();

            gameThread = null;

        private void Loop()
        {
            while (IsRunning)
            {
                Update();
                Thread.Sleep(1000 / Frames);
            }

            manualResetEvent.Set();
        }

    }
}
