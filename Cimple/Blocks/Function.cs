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

        public readonly List<(string, int)> Variables = new List<(string, int)>();

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

            Operations = Operation.ParseCodeBlock(this, (Dictionary<string, object>) contents["<code-block>_0"]);
        }

        public override string ToString()
        {
            var header = $"{Type} {Name} ({string.Join(", ", Operands.Select(o => $"u{o.Item2} {o.Item1}"))}) \n{{\n";
            var vars = string.Join("\n", Variables.Select(o => $"u{o.Item2} {o.Item1};"));
            var body = $"\n{string.Join("\n", Operations)} \n}}";
            return header + vars + body;
        }

        private int _offset;
        
        public int GetOffset(string name)
        {
            var find = Operands.FindIndex(si => si.Item1 == name);
            if (find!= -1)
                return _offset - (find + 1) * 8;
            
            find = Variables.FindIndex(si => si.Item1 == name);
            if (find != -1)
                return _offset - (Operands.Count + find + 1) * 8;
            
            throw new Exception("name not found");
        }

        public IEnumerable<string> Translate()
        {
            //label
            yield return $"{Name}:";
            //push arguments to stack
            for (var i = 0; i < Operands.Count; i++)
                yield return $"push {Program.RegParams[i]}";
            //allocate space for local variables
            yield return $"sub rsp, {Variables.Count * 8}";
            _offset = (Operands.Count + Variables.Count) * 8;

            foreach (var op in Operations.SelectMany(op => op.Translate()))
                yield return op;

            yield return ".return:";
            //remove arguments and local variables from stack
            yield return $"add rsp, {(Operands.Count + Variables.Count) * 8}";
            //return
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