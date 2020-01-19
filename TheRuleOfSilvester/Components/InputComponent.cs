using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Drawing;
using TheRuleOfSilvester.Runtime;
using TheRuleOfSilvester.Runtime.Interfaces;

namespace TheRuleOfSilvester.Components
{
    public sealed class InputComponent : IInputComponent
    {
        public IObservable<InputAction> InputActions { get; }

        public InputComponent(ConsoleInput consoleInput)
        {
            InputActions = consoleInput
                .ReceivedKeys
                .Select(k => k.Key)
                .Select(GetInputAction);
        }

        private InputAction GetInputAction(ConsoleKey lastKey)
        {
            switch (lastKey)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    return new InputAction(InputActionType.Up);
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    return new InputAction(InputActionType.Down);
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    return new InputAction(InputActionType.Left);
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    return new InputAction(InputActionType.Right);
                case ConsoleKey.NumPad0:
                case ConsoleKey.Q:
                    return new InputAction(InputActionType.StartAction);
                case ConsoleKey.R:
                    return new InputAction(InputActionType.RoundButton);
                case ConsoleKey.D1:
                    return new InputAction(InputActionType.RoundActionButton);
                case ConsoleKey.Escape:
                    return new InputAction(InputActionType.Back);
                default:
                    return new InputAction(InputActionType.Unknown);
            }
        }

    }
}
