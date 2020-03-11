using System.Collections.Generic;
using System.Linq;

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

        public static Dictionary<string, string> Conditions = new Dictionary<string, string>
        {
            ["=="] = "jne",
            ["<"] = "jge",
            ["<="] = "jg",
            [">"] = "jle",
            [">="] = "jl",
        };

        public override IEnumerable<string> Translate()
        {
            var cStart = Context.NextLabel();
            var cEnd = Context.NextLabel();
            yield return $".l{cStart}:";
            foreach (var e in Condition.Translate())
                yield return e;
            yield return $"{Conditions[((BinExpression)Condition).Operation]} .l{cEnd}";
            
            foreach (var op in Code.SelectMany(op => op.Translate()))
                yield return op;

            yield return $"jmp .l{cStart}";
            yield return $".l{cEnd}:";
        }
    }
}