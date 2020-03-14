using System.Collections.Generic;

namespace Cimple.Blocks
{
    public class ReturnOperator : Operation
    {
        public readonly Expression Value;
        
        public ReturnOperator(Function context) : base(context)
        {
        }
        
        public ReturnOperator(Function context, Expression value) : base(context)
        {
            Value = value;
        }
        
        public ReturnOperator(Function context, Dictionary<string, object> data) : base(context)
        {
            Value = Expression.ParseExpression(context, (Dictionary<string, object>)data["<expression>_0"]);
        }
        
        public override string ToString()
        {
            return $"return {Value};";
        }

        public override IEnumerable<string> Translate()
        {
            foreach (var op in Value.Translate())
                yield return op;
            if (Value.Result != "rax")
                yield return $"mov rax, {Value.Result}";
            if (Context.Operations[^1] != this)
                yield return "jmp .return";
        }
    }
}