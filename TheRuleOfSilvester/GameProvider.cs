using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester
{
    public sealed class GameProvider
    {

        public GameProvider()
        {
        }

        public Game CreateGame()
        {
            throw new NotImplementedException();
            //game.Run(60, isMultiplayer, playerName, x, y);
            //inputComponent.Start();

            //Console.CancelKeyPress += (s, e) => game.Stop();
            //game.Wait();

            //Console.Clear();
            //inputComponent.Stop();

            //game
            //    .Winners
            //    .FirstAsync()
            //    .Subscribe(s => Console.WriteLine("The winners are: "));

            //game
            //    .Winners
            //    .Subscribe(p =>
            //    {
            //        Console.WriteLine(p.Name);
            //    });

            //Console.WriteLine();
            //Console.WriteLine("Please press any key");
            //Console.ReadKey();
        }

    }
}
