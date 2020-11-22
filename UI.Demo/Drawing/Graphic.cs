using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Demo;

namespace TheRuleOfSilvester.UI.Drawing
{
    public class Graphic
    {
        private readonly Func<IObservable<GraphicInstruction.Write>, IObservable<GraphicInstruction.Write>> writeFunc;
        private readonly Func<IObservable<GraphicInstruction.WriteLine>, IObservable<GraphicInstruction.WriteLine>> writeLineFunc;
        private readonly Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> foregroundFunc;
        private readonly Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> backgroundFunc;
        private readonly Func<IObservable<GraphicInstruction.SetPosition>, IObservable<GraphicInstruction.SetPosition>> cursorPositionFunc;

        public Graphic(
            Func<IObservable<GraphicInstruction.Write>, IObservable<GraphicInstruction.Write>> write,
            Func<IObservable<GraphicInstruction.WriteLine>, IObservable<GraphicInstruction.WriteLine>> writeLine,
            Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> foreground,
            Func<IObservable<ConsoleColor>, IObservable<ConsoleColor>> background,
            Func<IObservable<GraphicInstruction.SetPosition>, IObservable<GraphicInstruction.SetPosition>> cursorPosition)
        {
            writeFunc = write;
            writeLineFunc = writeLine;
            foregroundFunc = foreground;
            backgroundFunc = background;
            cursorPositionFunc = cursorPosition;
        }

        public IObservable<GraphicInstruction.WriteLine> WriteLine(IObservable<GraphicInstruction.WriteLine> stringInput)
            => writeLineFunc(stringInput);
        public IObservable<GraphicInstruction.Write> Write(IObservable<GraphicInstruction.Write> stringInput)
            => writeFunc(stringInput);
        public IObservable<ConsoleColor> Foreground(IObservable<ConsoleColor> foreground)
            => foregroundFunc(foreground);
        public IObservable<ConsoleColor> Background(IObservable<ConsoleColor> background)
            => backgroundFunc(background);
        public IObservable<GraphicInstruction.SetPosition> CursorPosition(IObservable<GraphicInstruction.SetPosition> position)
            => cursorPositionFunc(position);
    }
}
