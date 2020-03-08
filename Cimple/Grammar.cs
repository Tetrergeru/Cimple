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

        private readonly string _start;
        
        public Grammar(Dictionary<string, List<List<string>>> grammar, string start)
        {
            _grammar = grammar;
            _start = start;
        }

        public (string, object, int) Match(List<Token> tokens, int start, string rool)
        {
            Console.WriteLine(rool);
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
            Console.WriteLine(key);
            return new Dictionary<string, object> { [$"{key}_0"] = value };
        }
    }
}