﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;

namespace Cimple.Blocks
{
    public class Operation
    {
        public readonly Function Context;

        public Operation(Function context)
        {
            Context = context;
        }

        public static List<Operation> ParseCodeBlock(Function Context, Dictionary<string, object> data)
        {
            
            var result = new List<Operation>();
            var operations =
                ((List<object>) data["<code-list>_0"])
                .Select(o => (Dictionary<string, object>) ((Dictionary<string, object>)o)["<operator>_0"]);
            foreach (var o in operations)
            {
                if (o.ContainsKey("<declaration-operator>_0"))
                {
                    var c = (Dictionary<string, object>)o["<declaration-operator>_0"];
                    Context.Variables.Add(( ((Token)c["Name_0"]).Text, int.Parse(((Token)c["Type_0"]).Text.Substring(1)) ));
                }
                else if (o.ContainsKey("<array-declaration-operator>_0"))
                {
                    var c = (Dictionary<string, object>)o["<array-declaration-operator>_0"];
                    var variable = ( ((Token)c["Name_0"]).Text, int.Parse(((Token)c["Type_0"]).Text.Substring(1)) );
                    var size = int.Parse(((Token) c["Const_0"]).Text);
                    for(var i = 1;i < size;i++)
                        Context.Variables.Add(("_", variable.Item2));
                    Context.Variables.Add(variable);
                    Context.Arrays.Add(variable.Item1);
                }
                else if (o.ContainsKey("<conditional-operator>_0"))
                    result.Add(new IfOperator(Context, (Dictionary<string, object>)o["<conditional-operator>_0"]));
                else if (o.ContainsKey("<while-cycle>_0"))
                    result.Add(new WhileOperator(Context, (Dictionary<string, object>)o["<while-cycle>_0"]));
                else if (o.ContainsKey("<return-operator>_0"))
                    result.Add(new ReturnOperator(Context, (Dictionary<string, object>)o["<return-operator>_0"]));
                else if (o.ContainsKey("<expression>_0"))
                    result.Add(Expression.ParseExpression(Context, (Dictionary<string, object>)o["<expression>_0"]));
            }

            return result;
        }

        public virtual IEnumerable<string> Translate()
        {
            return this switch
            {
                Expression e => e.Translate(),
                ReturnOperator ro => ro.Translate(),
                _ => throw new Exception()
            };
        }
    }
}