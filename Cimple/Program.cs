using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple
{
    public class Program
    {
        public static List<string> words = new List<string>
        {
            "==", "<=", ">=","+=", ">>", "<<", "||", "&&", "!=",
        };

        public static void Main()
        {
            var s = new StateMachine(new Parser(words).Parse);
            
            Console.WriteLine(string.Join(", ", s.ParseFile(@"> >> += ", "main.c").Select(s => $"{s}")));
        }
    }
}