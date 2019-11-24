using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class ConsoleInput
    {
        public IObservable<ConsoleKeyInfo> ReceivedKeys { get; set; }

        public ConsoleInput()
        {
            ReceivedKeys = CreateObservable()
                .Publish()
                .RefCount();
        }

        public  Task<string> ReadLine()
        {
            var builder = new StringBuilder();

            ReceivedKeys
                .Where(k=> k.Key == ConsoleKey.Enter || k.Key == ConsoleKey.Escape)
                .Subscribe(k =>
                {});

            ReceivedKeys
                .Where(k=> k.Key == ConsoleKey.Backspace)
                .Subscribe(k =>
                {
                    if (k.Key == ConsoleKey.Backspace)
                    {
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                        builder.Remove(builder.Length - 1, 1);
                    }
                });

            ReceivedKeys
                .Where(x=>x.Key != ConsoleKey.Enter || x.Key != ConsoleKey.Backspace || x.Key != ConsoleKey.Escape)
                .Select(k => k.KeyChar)
                .Do(c => Console.Write(c))
                .Subscribe(c => builder.Append(c));


            return Task.Run(() => {
                using var reader = new StreamReader(Console.OpenStandardInput());
                return reader.ReadLine();
            });
        }

        private IObservable<ConsoleKeyInfo> CreateObservable()
            => Observable.Create<ConsoleKeyInfo>((observer, token) => Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    observer.OnNext(Console.ReadKey(true));
                }
            }, token));

        private class ObservableStringReader
        {
            //TODO: Have fun
        }
    }
}
