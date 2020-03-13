using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimple.Blocks
{
    public class Expression : Operation
    {
        public string result;
        
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
            if (data.ContainsKey("<const-expression>_0"))
                return new ConstExpression(context, (Dictionary<string, object>)data["<const-expression>_0"]);
            
            throw new Exception("There is no such expression");
        }

        public new IEnumerable<string> Translate()
        {
            var translator = new Translator();
            var code = translator.Translate(this).ToList();
            code = Translator.Inline(code);
            code = translator.PutRegiters(code);
            foreach (var reg in translator.usedRegs)
                Context.UsedRegisters.Add(reg);
            result = translator.result;
            
            for (var i = 0; i < code.Count;)
            {
                if (code[i].left == code[i].right)
                    code.RemoveAt(i);
                else
                    i++;
            }
            
            foreach (var instr in code)
                if (instr.instr != "call" && instr.left[0] == '[' && char.IsDigit(instr.right[0]))
                {
                    instr.right = "DWORD " + instr.right;
                }

            return code.Select(l => $"{l}");
            //return this switch
            //{
            //    BinExpression b => b.Translate(),
            //    _ => throw new Exception()
            //};
        }
    }
}