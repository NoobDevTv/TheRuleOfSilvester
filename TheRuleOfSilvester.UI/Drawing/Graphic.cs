using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Drawing
{
    public class Graphic
    {
        private readonly Func<IObservable<string>, IObservable<string>> writeLineFunc;
        private readonly Func<IObservable<string>, IObservable<string>> writeFunc;
        private readonly Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> foregroundFunc;
        private readonly Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> backgroundFunc;
        private readonly Func<IObservable<Point>, IObservable<Point>> cursorPositionFunc;

        public Graphic(
            Func<IObservable<string>, IObservable<string>> write,
            Func<IObservable<string>, IObservable<string>> writeLine,
            Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> foreground,
            Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> background,
            Func<IObservable<Point>, IObservable<Point>> cursorPosition)
        {
            writeFunc = write;
            writeLineFunc = writeLine;
            foregroundFunc = foreground;
            backgroundFunc = background;
            cursorPositionFunc = cursorPosition;
        }

        public IObservable<string> WriteLine(IObservable<string> stringInput)
            => writeLineFunc(stringInput);
        public IObservable<string> Write(IObservable<string> stringInput)
            => writeFunc(stringInput);
        public IObservable<ConsoleColor> Foreground(IObservable<ConsoleColor> foreground)
            => foregroundFunc(foreground);
        public IObservable<ConsoleColor> Background(IObservable<ConsoleColor> background)
            => backgroundFunc(background);
        public IObservable<Point> CursorPosition(IObservable<Point> position)
            => cursorPositionFunc(position);
    }
}
