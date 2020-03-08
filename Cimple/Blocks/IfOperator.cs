using System.Collections.Generic;
using Cimple.Blocks;

namespace Cimple.Blocks
{
    public class IfOperator : Blocks.Operation
    {
        public readonly Expression Condition;

        public readonly List<Operation> CodeIf;
        
        public readonly List<Operation> CodeElse;
        
        public IfOperator(Function context) : base(context)
        {
        }
        
        public IfOperator(Function context, Expression condition, List<Operation> codeIf, List<Operation> codeElse) : base(context)
        {
            Condition = condition;
            CodeIf = codeIf;
            CodeElse = codeElse;
        }
        
        public IfOperator(Function context, Dictionary<string, object> data) : base(context)
        {
            Condition = Expression.ParseExpression(context, (Dictionary<string, object>)data["<expression>_0"]);
            CodeIf = ParseCodeBlock(context, (Dictionary<string, object>) data["<code-block>_0"]);
            CodeElse = ParseCodeBlock(context, (Dictionary<string, object>) data["<code-block>_1"]);
        }
        public override string ToString()
        {
            return $"if ({Condition})\n{{\n{string.Join("\n", CodeIf)}\n}}\nelse\n{{\n{string.Join("\n", CodeElse)}\n}}\n";
        }
    }
}