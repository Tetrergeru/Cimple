using System;
using System.Collections.Generic;

namespace Cimple.Blocks
{
    public class VarExpression : Expression
    {
        public readonly string Variable;
        
        public VarExpression(Function context) : base(context)
        {
        }
        
        public VarExpression(Function context, string variable) : base(context)
        {
            Variable = variable;
        }
        
        public VarExpression(Function context, Dictionary<string, object> data) : base(context)
        {
            Variable = ((Token)data["Name_0"]).Text;
        }
        
        public override string ToString()
        {
            return $"{Variable}";
        }
    }
}