﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cimple
{
    public class Program
    {
        public static List<string> words = new List<string> {
            "+=", "-=", "*=", "/=",
            ">>", "<<", "!=", "==", "<=", ">=",
            "||", "&&"
        };  
        public static void Print(object data, int tab)
        {
            switch (data)
            {
                case string s:
                    Console.WriteLine(s);
                    break;
                case Token t:
                    Console.WriteLine(t.Text);
                    break;
                case List<object> l:
                    if (l.Count == 0)
                        Console.WriteLine("<empty-list>");
                    else
                    {
                        Console.WriteLine();
                        Println(data, tab);
                    }
                    break;
                default:
                    Console.WriteLine();
                    Println(data, tab + 1);
                    break;
            }
        }

        public static void Println(object data, int tab = 0)
        {
            var tabs = string.Join("", Enumerable.Range(0, tab * 4).Select(x => ' '));
            switch (data)
            {
                case Dictionary<string, object> d:
                    foreach (var kv in d)
                    {
                        Console.Write($"{tabs}{kv.Key} => ");
                        Print(kv.Value, tab);
                    }
                    return;
                case List<object> l:
                    var i = 0;
                    foreach (var e in l)
                    {
                        Console.Write($"{tabs}   [{i}]:");
                        Print(e, tab + 1);
                        i++;
                    }
                    return;
            }
        }

        public static Grammar ReadGrammar(string fname)
        {
            var text = File.ReadAllText(fname);
            var rs = text
                .Split(";;")
                .Select(r => r.Split(new []{" ", "\n", "\r", "\t"}, StringSplitOptions.RemoveEmptyEntries))
                .Where( x => x.Length > 0)
                .ToList();

            var gr = rs.ToDictionary(
                r => r[0],
                r => string
                    .Join(' ', r.Skip(2))
                    .Split("|")
                    .Select(r => r.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .ToList());
            
            //Console.WriteLine(string.Join("\n", gr.Select(kv => 
            //    $"{kv.Key} => {string.Join(" | ", kv.Value.Select(x => string.Join(" ", x)))}")));
            
            return new Grammar(gr, rs.First()[0]);
        }

        public static void Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "--help" || args[0] == "-h")
            {
                Console.WriteLine(
                    "Use me like this:" 
                    + "Cimple <in>"
                    + ""
                    + "to compile file <in> into an executable");
                return;
            }

            var programName = args[0];
            var programNameBase = programName.Replace(".c", "");
            
            var s = new StateMachine(new Parser(words).Parse);
            var g = ReadGrammar("grammar.txt");
            if (!File.Exists(programName))
            {
                Console.WriteLine($"Файл {programName} не найден");
                return;
            }

            var program = s.ParseFile(File.ReadAllText(programName), programName);
            var parsed = g.Parse(program);

            var p = new Blocks.Program(parsed);
            
            File.WriteAllLines($"{programNameBase}.asm", p.Translate());
            var prc = System.Diagnostics.Process.Start("CMD.exe",$"/C nasm -f win64 {programNameBase}.asm");
            prc.WaitForExit();
            
            prc = System.Diagnostics.Process.Start("CMD.exe",$"/C golink /console {programNameBase}.obj kernel32.dll MSVCRT.dll Ws2_32.dll");
            prc.WaitForExit();
        }
    }
}