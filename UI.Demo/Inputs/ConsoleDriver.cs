﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using UI.Demo;

namespace TheRuleOfSilvester.UI.Inputs
{
    public static class ConsoleDriver
    {
        private readonly static IObservable<ConsoleKeyInfo> keyInfos;
        private static readonly IObservable<string> readLine;

        static ConsoleDriver()
        {
            keyInfos = CreateObservable()
                          .Publish()
                          .RefCount();

            readLine = CreateReadLine()
                          .Publish()
                          .RefCount();
        }

        public static IObservable<GraphicInstruction.WriteLine> WriteLine(IObservable<GraphicInstruction.WriteLine> stringInput)
            => stringInput.Do(s =>
            {
                Console.SetCursorPosition(s.Position.X, s.Position.Y);
                Console.WriteLine(s.Value);
            });

        public static IObservable<GraphicInstruction.Write> Write(IObservable<GraphicInstruction.Write> stringInput)
              => stringInput.Do(s =>
              {
                  Console.SetCursorPosition(s.Position.X, s.Position.Y);
                  Console.Write(s.Value);
              });

        public static IObservable<ConsoleColor> ForegroundColor(IObservable<ConsoleColor> forgroundColors)
            => forgroundColors
                .Do(c => Console.ForegroundColor = c);

        public static IObservable<ConsoleColor> BackgroundColor(IObservable<ConsoleColor> backgroundColors)
            => backgroundColors
                .Do(c => Console.BackgroundColor = c);

        public static IObservable<GraphicInstruction.SetPosition> SetCursorPosition(IObservable<GraphicInstruction.SetPosition> position)
            => position
                    .Select(p => p.Position)
                    .Do(p => { Console.CursorLeft = p.X; Console.CursorTop = p.Y; })
                    .Select(p => new GraphicInstruction.SetPosition(new(Console.CursorLeft, Console.CursorTop)));

        public static IObservable<ConsoleKeyInfo> ReadKey()
            => keyInfos;

        public static IObservable<string> ReadLine()
            => readLine;

        private static IObservable<string> CreateReadLine()
            => keyInfos
                .Window(keyInfos.Where(IsComplete))
                .SelectMany(keys =>
                    keys.Aggregate(new StringBuilder(), (builder, keyInfo) =>
                    {
                        if (IsAppend(keyInfo))
                            AppendKeyInfo(keyInfo, builder);

                        if (IsUndo(keyInfo) && Console.CursorLeft != 0 && builder.Length != 0)
                            UndoAppend(builder);

                        return builder;
                    }))
                .Select(builder => builder.ToString());

        private static void UndoAppend(StringBuilder builder)
        {
            Console.CursorLeft--;
            Console.Write(' ');
            Console.CursorLeft--;
            builder.Remove(builder.Length - 1, 1);
        }

        private static void AppendKeyInfo(ConsoleKeyInfo keyInfo, StringBuilder builder)
        {
            Console.Write(keyInfo.KeyChar);
            builder.Append(keyInfo.KeyChar);
        }


        private static IObservable<ConsoleKeyInfo> CreateObservable()
            => Observable.Create<ConsoleKeyInfo>((observer, token) => Task.Run(() =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    observer.OnNext(Console.ReadKey(true));
                }
            }, token));

        private static bool IsComplete(ConsoleKeyInfo keyInfo)
            => keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Escape;

        private static bool IsUndo(ConsoleKeyInfo keyInfo)
            => keyInfo.Key == ConsoleKey.Backspace;

        private static bool IsAppend(ConsoleKeyInfo keyInfo)
            => !IsComplete(keyInfo) && !IsUndo(keyInfo);
    }
}