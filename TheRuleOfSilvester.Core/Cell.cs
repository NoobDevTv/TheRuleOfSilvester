using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public abstract class Cell 
    {
        public const int HEIGHT = 3;
        public const int WIDTH = 5;
        public bool Invalid { get; set; }
        public string[,] Lines;


        public Cell()
        {
            Lines = new string[HEIGHT, WIDTH];
            Invalid = true;
        }
        
    }
}
