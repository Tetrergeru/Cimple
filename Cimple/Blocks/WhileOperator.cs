using System.Collections.Generic;

namespace Cimple.Blocks
{
    public class WhileOperator : Operation
    {
        public readonly Expression Condition;

        public readonly List<Operation> Code;
        
        public WhileOperator(Function context) : base(context)
        {
        }
        
        public WhileOperator(Function context, Expression condition, List<Operation> code) : base(context)
        {
            Condition = condition;
            Code = code;
        }
        
        public WhileOperator(Function context, Dictionary<string, object> data) : base(context)
        {
            Condition = Expression.ParseExpression(context, (Dictionary<string, object>)data["<expression>_0"]);
            Code = ParseCodeBlock(context, (Dictionary<string, object>) data["<code-block>_0"]);
        }
        
        public override string ToString()
        {
            return $"while ({Condition})\n{{{string.Join("\n", Code)}}}\n\n";
        }
    }
}