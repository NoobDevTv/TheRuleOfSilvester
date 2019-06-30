using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Interfaces;

namespace TheRuleOfSilvester
{
    public class InputComponent : IInputCompoment
    {
        public ConcurrentQueue<InputAction> InputActions { get; private set; }

        public bool Active { get; set; }

        private Task inputTask;
        private CancellationTokenSource source;

        public InputComponent()
        {
            Active = true;
            InputActions = new ConcurrentQueue<InputAction>();
        }

        internal void Start()
        {
            source = new CancellationTokenSource();

            inputTask = new Task(async () => await InternalListen(source.Token), TaskCreationOptions.LongRunning);

            inputTask.Start(TaskScheduler.Default);
        }

        internal void Stop()
        {
            source?.Cancel();
            source?.Dispose();
            source = null;

            inputTask?.Dispose();
            inputTask = null;
        }

        private Task InternalListen(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ConsoleKey lastKey = Console.ReadKey(true).Key;

                if (lastKey == ConsoleKey.Escape)
                    InputActions.Enqueue(new InputAction(InputActionType.EndGame));

                if (!Active)
                    continue;

                switch (lastKey)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        InputActions.Enqueue(new InputAction(InputActionType.Up));
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        InputActions.Enqueue(new InputAction(InputActionType.Down));
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        InputActions.Enqueue(new InputAction(InputActionType.Left));
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        InputActions.Enqueue(new InputAction(InputActionType.Right));
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.Q:
                        InputActions.Enqueue(new InputAction(InputActionType.StartAction));
                        break;
                    case ConsoleKey.R:
                        InputActions.Enqueue(new InputAction(InputActionType.RoundButton));
                        break;
                    case ConsoleKey.D1:
                        InputActions.Enqueue(new InputAction(InputActionType.RoundActionButton));
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            source?.Cancel();

            source?.Dispose();

            source = null;
            inputTask = null;
        }
    }
}
