using System.Collections.Generic;

namespace Cimple.Blocks
{
    public class ConstExpression : Expression
    {
        public readonly string Const;
        
        public ConstExpression(Function context) : base(context)
        {
        }
        
        public ConstExpression(Function context, string cnst) : base(context)
        {
            Const = cnst;
        }
        
        public ConstExpression(Function context, Dictionary<string, object> data) : base(context)
        {
            Const = ((Token)data["Const_0"]).Text;
        }
        
        public override string ToString()
        {
            return $"{Const}";
        }
    }
}