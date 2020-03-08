using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple.Blocks
{
    public class Program
    {
        private Dictionary<string, Function> _funcs = new Dictionary<string, Function>();
        
        public Program(Dictionary<string, Function> funcs)
        {
            _funcs = funcs;
        }

        public Program(Dictionary<string, object> program)
        {
            if (program["<program-list>_0"] is List<object> list)
            {
                foreach(var e in list)
                    if (e is Dictionary<string, object> data)
                    {
                        var func = (Dictionary<string, object>) data["<function>_0"];
                        _funcs[((Token)func["Name_0"]).Text] = new Function(this, func);
                    }
            }
            else
            {
                throw new Exception("Parsed incorrectly: no program-list ar it is not List");
            }
        }

        public override string ToString()
        {
            return string.Join("\n", _funcs.Values);
        }
    }
}