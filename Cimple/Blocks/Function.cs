using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cimple.Blocks
{
    public class Function
    {
        public readonly Program Context;

        public readonly string Name;

        public readonly string Type;

        public readonly List<(string, int)> Operands = new List<(string, int)>();

        public readonly List<Operation> Operations = new List<Operation>();

        public readonly List<(string, int)>Variables = new List<(string, int)>();

        public readonly HashSet<string> UsedRegisters = new HashSet<string>();
        
        public readonly List<string> Arrays = new List<string>();
        
        public Function(Program context, Dictionary<string, object> contents)
        {
            Context = context;

            Name = (contents["Name_0"] as Token)?.Text ??
                   throw new Exception("Parsed incorrectly: wrong name within function");

            Type = (contents["Type_0"] as Token)?.Text ??
                   throw new Exception("Parsed incorrectly: wrong type within function");

            Operands = new List<(string, int)>();
            var operands = contents["<variable-list>_0"] as List<object> ??
                           throw new Exception("Parsed incorrectly: wrong parameters within function");
            
            foreach (Dictionary<string, object> o in operands)
                Operands.Add((((Token) o["Name_0"]).Text, int.Parse(((Token) o["Type_0"]).Text.Substring(1))));

            if (Name == "main")
            {
                Variables.Add(("__STACK_START", 64));
                Operations.Add(new CallExpression(this,
                    new List<Expression>
                    {
                        new BinExpression(this,
                            new UnExpression(this, new VarExpression(this, "__STACK_START"), "&"), 
                            new ConstExpression(this, "10000"),
                            "-"),
                        new ConstExpression(this, "10176"),
                        new ConstExpression(this, "0x40"),
                        new UnExpression(this, new VarExpression(this, "__STACK_START"), "&")
                    },
                    "VirtualProtect"));
            }

            Operations.AddRange(Operation.ParseCodeBlock(this, (Dictionary<string, object>) contents["<code-block>_0"]));
        }

        public override string ToString()
        {
            var header = $"{Type} {Name} ({string.Join(", ", Operands.Select(o => $"u{o.Item2} {o.Item1}"))}) \n{{\n";
            var vars = string.Join("\n", Variables.Select(o => $"u{o.Item2} {o.Item1};"));
            var body = $"\n{string.Join("\n", Operations)} \n}}";
            return header + vars + body;
        }

        private int _offset;

        public void Push(int bytes)
        {
            _offset += bytes;
        }
        
        public void Pop(int bytes)
        {
            _offset -= bytes;
        }

        public int GetOffset(string name)
        {
            var find = Operands.FindIndex(si => si.Item1 == name);
            if (find!= -1)
                return _offset - (find + 1) * 8;
            
            find = Variables.FindIndex(si => si.Item1 == name);
            if (find != -1)
                return  _offset - (Operands.Count + find + 1) * 8;
            
            throw new Exception($"name not found: [{name}]");
        }

        private int _label = 0;
        
        public int NextLabel()
        {
            return _label++;
        }

        public static HashSet<string> regToSave = new HashSet<string>
        {
            "rbx", "rdi", "rsi", "r12", "r13", "r14", "r15"
        };
        
        public IEnumerable<string> Translate()
        {
            //label
            yield return $"{Name}:";
            //push arguments to stack
            for (var i = 0; i < Operands.Count; i++)
                yield return $"push {Program.RegParams[i]}";
            //allocate space for local variables
            if (Variables.Count > 0)
                yield return $"sub rsp, {Variables.Count * 8}";
            _offset = (Operands.Count + Variables.Count) * 8;

            var _ = Operations.SelectMany(op => op.Translate()).ToList();
            
            //save registers
            foreach (var reg in UsedRegisters.OrderBy(x => x))
                if (regToSave.Contains(reg))
                {
                    yield return $"push {reg}";
                    Push(8);
                }

            foreach (var op in Operations.SelectMany(op => op.Translate()))
                yield return op;

            //return
            yield return ".return:";
            //load registers
            foreach (var reg in UsedRegisters.OrderBy(x => x).Reverse())
                if (regToSave.Contains(reg))
                {
                    yield return $"pop {reg}";
                    Push(8);
                }
            //remove arguments and local variables from stack
            if (Operands.Count + Variables.Count > 0)
                yield return $"add rsp, {(Operands.Count + Variables.Count) * 8}";
            if (Name == "main")
            {
                yield return "xor rcx, rcx";
                yield return "call ExitProcess";
            }
            else
                yield return "ret";
            yield return "";
        }
    }
}