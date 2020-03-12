using System;
using System.Collections.Generic;

namespace Cimple.Blocks
{
    public class UnExpression : Expression
    {
        public Expression Operand { get; }

        public string Operation { get; }
        
        public UnExpression(Function context) : base(context)
        {
        }

        public UnExpression(Function context, Expression operand, string op) : base(context)
        {
            Operand = operand;
            Operation = op;
        }
        
        public UnExpression(Function context, Dictionary<string, object> data) : base(context)
        {
            Operand = Expression.ParseExpression(context, (Dictionary<string, object>) data["<expression>_0"]);
            Operation = ((Token)data["Operator_0"]).Text;
        }
        
        public override string ToString()
        {
            return $"({Operation} {Operand})";
        }

        public new IEnumerable<string> Translate()
        {
            var vbl = ((VarExpression) Operand).Variable;
            var off = Context.GetOffset(vbl);
            switch (Operation)
            {
                case "&":
                {
                    yield return "mov rax, rsp";
                    yield return $"add rax, {off}";
                    break;
                }
                case "*":
                {
                    yield return $"mov rbx, [rsp + {off}]";
                    break;
                }
                default:
                    throw new Exception("Dunno such operations");
            }
        }
    }
}
