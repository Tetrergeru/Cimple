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

        public static void Main()
        {
            var s = new StateMachine(new Parser(words).Parse);
            var gr1 =
                new Dictionary<string, List<List<string>>>
                {
                    ["<function>"] = new List<List<string>>
                    {
                        new List<string> {"<Type>", "Name", "(", ")", "{", "<code block>", "}"}
                    },
                    ["<code block>"] = new List<List<string>>
                    {
                        new List<string> {"<expression>", ";"}
                    },
                    ["<expression>"] = new List<List<string>>
                    {
                        new List<string> {"<Name>"},
                        new List<string> {"(", "<expression>", "<Operator>", "<expression>", ")"}
                    },
                };
            var g = new Grammar( new Dictionary<string, List<List<string>>>
                {
                    ["<function>"] = new List<List<string>>
                    {
                        new List<string> {"<Name>", "<function>"},
                        new List<string> {}
                    },
                },
                "<function>");
            
            var code = @"u64 foo() {(x = y);}";
            var program = s.ParseFile("a a a a a", "main.c");
            var parsed = g.Parse(program);
            Print(parsed);
        }
    }
}