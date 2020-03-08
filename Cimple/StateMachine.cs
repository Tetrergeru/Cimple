using System;
using System.Collections.Generic;
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

        public IEnumerable<string> ParseTokens(string program)
        {
            var state = 0;
            var result = new StringBuilder();
            foreach (var ch in program)
            {
                if ((char.IsWhiteSpace(ch) || ch == '\n') && result.Length != 0)
                {
                    yield return result.ToString();
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
                    yield return result.ToString();
                    result = new StringBuilder();
                }
                result.Append(ch);
            }

            if (result.Length != 0)
                yield return result.ToString();
        }
    }
}