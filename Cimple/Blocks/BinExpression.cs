using System;
using System.Collections.Generic;

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
            Operation = ((Token)data["Operator_0"]).Text;
        }

        public override string ToString()
        {
            return $"{Left} {Operation} {Right}";
        }
    }
}