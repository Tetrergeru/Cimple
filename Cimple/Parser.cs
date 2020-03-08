using System.Collections.Generic;
using System.Linq;

namespace Cimple
{
    public class Parser
    {
        private Dictionary<char, int> _id;
        private List<string> _words;

        public Parser(List<string> words)
        {
            _id = words.Select(s => s[0]).Distinct().Select((c, i) => (c, i))
                .ToDictionary(c => c.Item1, c => c.Item2 + 2);
            _words = words;
        }

        public (int, bool) Parse((char, int) state)
        {
            var (ch, st) = state;
            
            if (char.IsDigit(ch) || char.IsLetter(ch) || ch == '_')
                return (1, st != 1);

            if (_words.Any(w => w[1] == ch && _id.ContainsKey(w[0]) && _id[w[0]] == st))
                return (0, false);
            
            return _id.ContainsKey(ch) ? (_id[ch], true) : (0, true);
        }
    }
}