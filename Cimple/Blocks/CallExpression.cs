using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple.Blocks
{
    public class CallExpression : Expression
    {
        public List<Expression> Operands { get; }

        public string Function { get; }
        
        public CallExpression(Function context) : base(context)
        {
        }

        public CallExpression(Function context, List<Expression> operands, string function) : base(context)
        {
            Operands = operands;
            Function = function;
        }
        public CallExpression(Function context, Dictionary<string, object> data) : base(context)
        {
            Function = ((Token) data["Name_0"]).Text;
            Operands = ((List<object>) data["<argument-list>_0"])
                .Select( o => (Dictionary<string, object>) o)
                .Select(o => Expression.ParseExpression(context, (Dictionary<string, object>)o["<expression>_0"]))
                .ToList();
            
            if (Function == "printd")
                Operands.Insert(0, new ConstExpression(context, "fmt"));
        }
        public override string ToString()
        {
            return $"{Function}({string.Join(", ", Operands)})";
        }

        public new IEnumerable<string> Translate()
        {
            if (Function == "printf" || Function == "printd")
            {
                yield return "sub rsp, 40";
                Context.Push(40);
            }

            for (var i = 0; i < Operands.Count; i++)
                yield return $"mov {Program.RegParams[i]}, {Operands[i]}";
            var fun = Function == "printd" ? "printf" : Function;
            yield return $"call {fun}";
            
            if (Function == "printf" || Function == "printd")
            {
                yield return "add rsp, 40";
                Context.Pop(40);
            }
        }
    }
}