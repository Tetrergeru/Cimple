using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cimple
{
    public class Program
    {
        public static List<string> words = new List<string>
        {
            "==", "<=", ">=","+=", ">>", "<<", "||", "&&", "!=",
        };

        public static void Print(object data, int tab = 0)
        {
            var tabs = string.Join("", Enumerable.Range(0, tab * 3).Select(x => ' '));
            switch (data)
            {
                case Dictionary<string, object> d:
                    foreach (var kv in d)
                    {
                        Console.Write($"{tabs}{kv.Key} => ");
                        switch (kv.Value)
                        {
                            case string s:
                                Console.WriteLine(s);
                                break;
                            case Token t:
                                Console.WriteLine(t.Text);
                                break;
                            default:
                                Console.WriteLine();
                                Print(kv.Value, tab + 1);
                                break;
                        }
                    }
                    return;
            }
        }

        public static Grammar ReadGrammar(string fname)
        {
            var text = File.ReadAllText(fname);
            var rs = text
                .Split(";;")
                .Select(r => r.Split(new []{" ", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries))
                .Where( x => x.Length > 0)
                .ToList();

            var gr = rs.ToDictionary(
                r => r[0],
                r => string
                    .Join(' ', r.Skip(2))
                    .Split("|")
                    .Select(r => r.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .ToList());
            
            Console.WriteLine(string.Join("\n", gr.Select(kv => 
                $"{kv.Key} => {string.Join(" | ", kv.Value.Select(x => string.Join(" ", x)))}")));
            
            return new Grammar(gr, rs.First()[0]);
        }

        public static void Main()
        {
            var s = new StateMachine(new Parser(words).Parse);
            var g = ReadGrammar("grammar.txt");
            
            var code = @"
u64 foo()
{
   (x = y);
   (y = z);
}";

            var program = s.ParseFile(code, "main.c");
            //Console.WriteLine(string.Join(", ", program.Select(t => t.Text)));
            
            var parsed = g.Parse(program);
            Print(parsed);
        }
    }
}