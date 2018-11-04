using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    public class InputComponent : IInputCompoment
    {
        public ConcurrentQueue<InputAction> InputActions { get; private set; }

        public bool Active { get; set; }
        
        private Thread inputThread;
        private bool running;

        public InputComponent()
        {
            Active = true;
            InputActions = new ConcurrentQueue<InputAction>();
        }

        internal void Start()
        {
            running = true;
            inputThread = new Thread(InternalListen)
            {
                IsBackground = true,
                Name = "Input Thread"
            };

            inputThread.Start();
        }

        internal void Stop()
        {
            running = false;
        }

        private void InternalListen()
        {
            while (running)
            {
                var lastKey = Console.ReadKey(true).Key;

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

                    case ConsoleKey.Escape:
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
