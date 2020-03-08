using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cimple
{
    public class Grammar
    {
        private readonly Dictionary<string, List<List<string>>> _grammar;

        private readonly List<string> _lists;
        
        private readonly string _start;
        
        public Grammar(Dictionary<string, List<List<string>>> grammar, string start)
        {
            _grammar = grammar;
            _start = start;
            _lists = grammar
                .Where(kv => kv.Value.Count == 2)
                .Where(kv => kv.Value[1].Count == 0 && kv.Value[0][1] == kv.Key)
                .Select(kv => kv.Key)
                .ToList();
            
            Console.WriteLine(string.Join(", ", _lists));
        }

        // TODO: Rewrite, it probably doesn't work
        private void UnwrapLists(Dictionary<string, object> program)
        {
            foreach (var l in _lists)
            {
                if (!program.ContainsKey($"{l}_0")) 
                    continue;
                
                var i = 1;
                var next = program[$"{l}_0"];
                while (next is Dictionary<string, object> d && d.ContainsKey($"{l}_0"))
                {
                    program[$"{l}_{i}"] = next;
                    next = d[$"{l}_0"];
                    i++;
                }
                program.Remove($"{l}_{i - 1}");
            }
            foreach (var kv in program)
                if (kv.Value is Dictionary<string, object> d)
                    UnwrapLists(d);
        }

        private (string, object, int) Match(List<Token> tokens, int start, string rool)
        {
            if (rool.Length > 2 && rool[0] == '<' && rool.Last() == '>')
            {
                var text = rool.Substring(1, rool.Length - 2);
                
                if (Enum.TryParse<TokenType>(text, out var roolTType))
                    return tokens.Count > start && tokens[start].TType == roolTType 
                        ? ($"{roolTType}", tokens[start], start + 1) 
                        : (null, null, -1);

                foreach (var rs in _grammar[rool])
                {
                    var result = new Dictionary<string, object>();
                    var newStart = start;
                    var flag = true;
                    foreach (var r in rs)
                    {
                        string key;
                        object value;
                        (key, value, newStart) = Match(tokens, newStart, r);
                        if (newStart >= 0)
                        {
                            if (key == null)
                                continue;
                            
                            var i = 0;
                            while (result.ContainsKey($"{key}_{i}"))
                                i++;
                            result[$"{key}_{i}"] = value;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                        return (rool, result, newStart);
                }

                return (null, null, -1);
            }

            if (tokens[start].Text == rool)
                return (null, null, start + 1);
            
            return (null, null, -1);
        }

        public Dictionary<string, object> Parse(IEnumerable<Token> program)
        {
            var (key, value, i) = Match(program.ToList(), 0, _start);
            var parsedProgram =  new Dictionary<string, object> { [$"{key}_0"] = value };
            UnwrapLists(parsedProgram);
            return parsedProgram;
        }
    }
}