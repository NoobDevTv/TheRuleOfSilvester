using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.Drawing
{
    public sealed class ConsoleInput
    {
        public IObservable<ConsoleKeyInfo> ReceivedKeys { get; set; }


        public ConsoleInput()
        {
            var connectable = CreateObservable()
                                .Publish();
            connectable.Connect();
            ReceivedKeys = connectable;
        } 

        public async Task<string> ReadLine(CancellationToken token)
        {
            using var builder = new ObservableStringBuilder(ReceivedKeys);
            return await builder.ReadLine(token);
        }
        public async Task<string> ReadLine(string defaultValue, CancellationToken token, bool intercept = true)
        {
            using var builder = new ObservableStringBuilder(defaultValue, ReceivedKeys);

            if (!intercept)
                Console.Write(defaultValue);

            return await builder.ReadLine(token);
        }

        public async Task<ConsoleKeyInfo> ReadKey(bool intercept, CancellationToken token)
        {
            using var builder = new ObservableStringBuilder(ReceivedKeys);
            return await builder.ReadKey(intercept, token);
        }

        private IObservable<ConsoleKeyInfo> CreateObservable()
            => Observable.Create<ConsoleKeyInfo>((observer, token) => Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    observer.OnNext(Console.ReadKey(true));
                }
            }, token));

        private sealed class ObservableStringBuilder : IDisposable
        {
            private readonly StringBuilder stringBuilder;
            private readonly IObservable<ConsoleKeyInfo> closeKeys;
            private readonly IObservable<ConsoleKeyInfo> backspace;
            private readonly IObservable<ConsoleKeyInfo> appendkeys;
            private readonly IObservable<ConsoleKeyInfo> keys;

            private readonly SerialDisposable disposables;
            private readonly ManualResetEventSlim resetEvent;

            public ObservableStringBuilder(IObservable<ConsoleKeyInfo> observableKeys)
            {
                disposables = new SerialDisposable();
                stringBuilder = new StringBuilder();
                resetEvent = new ManualResetEventSlim();

                keys = observableKeys;
                closeKeys = observableKeys.Where(k => k.Key == ConsoleKey.Enter || k.Key == ConsoleKey.Escape);
                backspace = observableKeys.Where(k => k.Key == ConsoleKey.Backspace);
                appendkeys = observableKeys.Where(x => x.Key != ConsoleKey.Enter && x.Key != ConsoleKey.Backspace && x.Key != ConsoleKey.Escape);
            }
            public ObservableStringBuilder(string defaultValue, IObservable<ConsoleKeyInfo> observableKeys) : this(observableKeys)
            {
                stringBuilder.Append(defaultValue);
            }

            public Task<string> ReadLine(CancellationToken token)
            {
                resetEvent.Reset();
                var close = closeKeys.Subscribe(Close);
                var undo = backspace.Subscribe(Undo);
                var append = appendkeys.Subscribe(Append);
                disposables.Disposable = new CompositeDisposable { close, undo, append };

                return Task.Run(() =>
                {
                    resetEvent.Wait(token);
                    Console.WriteLine();
                    return stringBuilder.ToString();
                }, token);
            }

            public async Task<ConsoleKeyInfo> ReadKey(bool intercept, CancellationToken token)
            {
                ConsoleKeyInfo consoleKey = default;
                resetEvent.Reset();
                using var dispose = keys.Subscribe(k =>
                 {
                     if (!intercept)
                         Console.Write(k.KeyChar);

                     consoleKey = k;
                     resetEvent.Set();
                 });

                return await Task.Run(() =>
                {
                    resetEvent.Wait(token);
                    return consoleKey;
                }, token);
            }

            public void Dispose()
            {
                stringBuilder.Clear();

                disposables.Dispose();
                resetEvent.Dispose();
            }

            private void Append(ConsoleKeyInfo obj)
            {
                Console.Write(obj.KeyChar);
                stringBuilder.Append(obj.KeyChar);
            }

            private void Undo(ConsoleKeyInfo obj)
            {
                if (Console.CursorLeft == 0 || stringBuilder.Length == 0)
                    return;
                Console.CursorLeft--;
                Console.Write(" ");
                Console.CursorLeft--;
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            private void Close(ConsoleKeyInfo obj)
            {
                resetEvent.Set();
            }
        }

    }
}
