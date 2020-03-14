using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cimple
{
    public class StateMachine
    {
        private Func<(char, int), (int, bool)> _program;

        public StateMachine(Dictionary<(char, int), (int, bool)> program)
            => _program = c => program.ContainsKey((c.Item1, c.Item2)) ? program[(c.Item1, c.Item2)] : (0, true);

        public StateMachine(Func<(char, int), (int, bool)> program)
            => _program = program;

        public IEnumerable<Token> ParseFile(string program, string fname = "")
            => program
                .Split("\n").Where(l => !l.TrimStart().StartsWith("//"))
                .SelectMany((line, lineNumber) => ParseLine(line, fname, lineNumber));

        public IEnumerable<Token> ParseLine(string line, string fname, int lineNumber)
        {
            var state = 0;
            var charNumber = 0;
            var result = new StringBuilder();
            foreach (var ch in line)
            {
                if ((char.IsWhiteSpace(ch) || ch == '\n') && result.Length != 0)
                {
                    yield return new Token(result.ToString(), fname, lineNumber, charNumber);
                    result = new StringBuilder();
                }

                if (char.IsWhiteSpace(ch) || ch == '\n')
                {
                    state = 0;
                    continue;
                }

                bool end;
                (state, end) =_program((ch, state));
                
                if (end && result.Length != 0)
                {
                    yield return new Token(result.ToString(), fname, lineNumber, charNumber);
                    result = new StringBuilder();
                }
                result.Append(ch);
                
                charNumber++;
            }

            if (result.Length != 0)
                yield return new Token(result.ToString(), fname, lineNumber, charNumber);
        }
    }
}