using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Inputs
{
    public static class ConsoleDriver
    {
        private readonly static IObservable<ConsoleKeyInfo> keyInfos;

        static ConsoleDriver()
        {
            keyInfos = CreateObservable()
                          .Publish()
                          .RefCount();
        }

        public static IObservable<string> WriteLine(IObservable<string> strings)
            => strings.Do(Console.WriteLine);

        public static IObservable<string> Write(IObservable<string> strings)
            => strings.Do(Console.Write);

        public static IObservable<ConsoleColor> ForGroundColor(IObservable<ConsoleColor> forgroundColors)
            => forgroundColors
                .Do(c => Console.ForegroundColor = c);

        public static IObservable<ConsoleColor> BackGroundColor(IObservable<ConsoleColor> backgroundColors)
            => backgroundColors
                .Do(c => Console.BackgroundColor = c);

        public static IObservable<(int x, int y)> SetCursorPosition(IObservable<(int x, int y)> cursorPosition)
            => cursorPosition
                    .Do(p => { Console.CursorLeft = p.x; Console.CursorTop = p.y; })
                    .Select(p => (Console.CursorLeft, Console.CursorTop));

        public static IObservable<ConsoleKeyInfo> ReadKey()
            => keyInfos;

        public static IObservable<string> ReadLine()
            => Observable.Create<string>(observer =>
            {
                StringBuilder builder = new();
                var subj = new Subject<string>();
                var sub = keyInfos
                            .Do(keyInfo =>
                            {
                                if (IsComplete(keyInfo))
                                    CompleteString(builder, subj);

                                if (IsAppend(keyInfo))
                                    AppendKeyInfo(keyInfo, builder);

                                if (IsUndo(keyInfo) && Console.CursorLeft != 0 && builder.Length != 0)
                                    UndoAppend(builder);
                            })
                            .Subscribe();

                return StableCompositeDisposable.Create(subj, sub);

            });

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

        private static void CompleteString(StringBuilder builder, Subject<string> subj)
        {
            subj.OnNext(builder.ToString());
            builder.Clear();
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
            => keyInfo.Key == ConsoleKey.Enter;
    }
}
