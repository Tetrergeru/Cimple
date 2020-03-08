using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple.Blocks
{
    public class Expression : Operation
    {
        public Expression(Function context) : base(context)
        {
        }

        public static Expression ParseExpression(Function context, Dictionary<string, object> data)
        {
            if (data.ContainsKey("<bin-expression>_0"))
                return new BinExpression(context, (Dictionary<string, object>)data["<bin-expression>_0"]);
            if (data.ContainsKey("<un-expression>_0"))
                return new UnExpression(context, (Dictionary<string, object>)data["<un-expression>_0"]);
            if (data.ContainsKey("<call-expression>_0"))
                return new CallExpression(context, (Dictionary<string, object>)data["<call-expression>_0"]);
            if (data.ContainsKey("<name-expression>_0"))
                return new VarExpression(context, (Dictionary<string, object>)data["<name-expression>_0"]);
            
            throw new Exception("There is no such expression");
        }
    }
}