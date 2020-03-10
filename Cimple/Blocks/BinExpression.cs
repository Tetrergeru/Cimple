using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple.Blocks
{
    public class BinExpression : Expression
    {
        public Expression Left { get; }

        public Expression Right { get; }

        public string Operation { get; }

        public BinExpression(Function context) : base(context)
        {
        }

        public BinExpression(Function context, Expression left, Expression right, string op) : base(context)
        {
            Left = left;
            Right = right;
            Operation = op;
        }

        public BinExpression(Function context, Dictionary<string, object> data) : base(context)
        {
            Left = Expression.ParseExpression(context, (Dictionary<string, object>) data["<expression>_0"]);
            Right = Expression.ParseExpression(context, (Dictionary<string, object>) data["<expression>_1"]);
            Operation = ((Token) data["Operator_0"]).Text;
        }

        public override string ToString()
        {
            return $"{Left} {Operation} {Right}";
        }

        private static Dictionary<string, string> OpToInstr = new Dictionary<string, string>
        {
            ["="] = "mov",
            ["+="] = "add",
            ["-="] = "dec",
        };

        public override IEnumerable<string> Translate()
        {
            var result = new List<string>();
            result.AddRange(Right switch
            {
                VarExpression ve => new[] {$"mov rax, {ve}"},
                ConstExpression ce => new[] {$"mov rax, {int.Parse(ce.Const)}"},
                CallExpression ce => ce.Translate(),
                _ => throw new Exception($"{Right.GetType()}")
            });

            var instruction = OpToInstr.ContainsKey(Operation)
                ? OpToInstr[Operation]
                : throw new Exception();

            result.Add($"{instruction} {Left}, rax");

            return result;
        }
    }
}