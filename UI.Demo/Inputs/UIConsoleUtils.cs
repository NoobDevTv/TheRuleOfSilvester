using System;
using System.Reactive.Linq;
using System.Text;

namespace UI.Demo.Inputs
{
    internal static class UIConsoleUtils
    {
        public static IObservable<string> StringBuilder(IObservable<ConsoleKeyInfo> keyInfos, Func<ConsoleKeyInfo, bool> isComplete = null)
            => StringBuilder(keyInfos, null, isComplete);

        public static IObservable<string> StringBuilder(IObservable<ConsoleKeyInfo> keyInfos, string startValue, Func<ConsoleKeyInfo, bool> isComplete = null)
            => keyInfos
                .Window(keyInfos.Where(isComplete ?? IsComplete))
                .SelectMany(keys =>
                    keys.Aggregate(new StringBuilder(startValue), (builder, keyInfo) =>
                    {
                        if (IsAppend(keyInfo))
                            AppendKeyInfo(keyInfo, builder);

                        if (IsUndo(keyInfo) && builder.Length != 0)
                            UndoAppend(builder);

                        return builder;
                    }))
                .Select(builder => builder.ToString());

        public static bool IsComplete(ConsoleKeyInfo keyInfo)
            => keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Escape;

        public static bool IsUndo(ConsoleKeyInfo keyInfo)
            => keyInfo.Key == ConsoleKey.Backspace;

        public static bool IsAppend(ConsoleKeyInfo keyInfo)
            => !IsComplete(keyInfo) && !IsUndo(keyInfo);

        private static void UndoAppend(StringBuilder builder)
        {
            builder.Remove(builder.Length - 1, 1);
        }

        private static void AppendKeyInfo(ConsoleKeyInfo keyInfo, StringBuilder builder)
        {
            builder.Append(keyInfo.KeyChar);
        }
    }
}
