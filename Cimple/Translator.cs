using System;
using System.Collections.Generic;
using System.Linq;
using Cimple.Blocks;

namespace Cimple
{
    public class AsmLine
    {
        public string instr;
        public string left;
        public string right;

        public AsmLine(string i, string l, string r)
        {
            instr = i;
            left = l;
            right = r;
        }

        public override string ToString()
            => right != null ? $"{instr} {left}, {right}" : $"{instr} {left}";
    }
    
    public class Translator
    {
        private int _regUsed;

        private string NewReg() => $"#{++_regUsed}#";
        public Translator()
        {
        }

        public static readonly List<string> RegParams = new List<string> {"rcx", "rdx", "r8", "r9"};
        
        private static HashSet<string> Assigners = new HashSet<string>
        {
            "=", "+=", "-=",
        };
        
        private static HashSet<string> Comparators = new HashSet<string>
            {
                "==", "<=", "<", ">",">=","!="
            };
        
        private static Dictionary<string, string> OpToInstr = new Dictionary<string, string>
        {
            ["="] = "mov",
            ["|"] = "or",
            ["+"] = "add",
            ["+="] = "add",
            ["-"] = "sub",
            ["-="] = "sub",
            ["=="] = "sete",
            ["!="] = "setne",
            ["<"] = "setl",
            ["<="] = "setle",
            [">"] = "setg",
            [">="] = "setge",
        };

        public IEnumerable<AsmLine> TranslateAssign(BinExpression be)
        {
            string leftOp;
            switch (be.Left)
            {
                case UnExpression ue when ue.Operation == "*":
                {
                    foreach (var op in Translate(ue.Operand))
                        yield return op;
                    leftOp = $"[#{_regUsed}#]";
                    break;
                }
                case VarExpression ve:
                    leftOp = $"{ve}";
                    break;
                default:
                    throw new Exception("lvalue incorrect");
            }

            foreach (var op in Translate(be.Right))
                yield return op;
            var rightResReg = _regUsed;

            yield return new AsmLine(OpToInstr[be.Operation], leftOp, $"#{rightResReg}#");
        }

        public IEnumerable<AsmLine> Translate(BinExpression be)
        {
            foreach (var op in Translate(be.Right))
                yield return op;
            var rightResReg = _regUsed;
            
            foreach (var op in Translate(be.Left))
                yield return op;
            var leftResReg = _regUsed;

            if (Comparators.Contains(be.Operation))
            {
                yield return new AsmLine("cmp", $"#{leftResReg}#", $"#{rightResReg}#");
                yield return new AsmLine(OpToInstr[be.Operation],NewReg(), null);
            }
            else
                yield return new AsmLine(OpToInstr[be.Operation], $"#{leftResReg}#", $"#{rightResReg}#");
        }

        public IEnumerable<AsmLine> Translate(CallExpression ce)
        {
            if (ce.Function == "printf" || ce.Function == "printd" || ce.Function == "VirtualAlloc")
            {
                yield return new AsmLine("sub","rsp", "40");
                ce.Context.Push(40);
            }
            
            var opRegs = new List<int>();
            foreach (var op in ce.Operands)
            {
                foreach (var ex in Translate(op))
                    yield return ex;
                opRegs.Add(_regUsed);
            }
            for(var i = 0;i<opRegs.Count;i++)
                yield return new AsmLine("mov", $"{RegParams[i]}", $"#{opRegs[i]}#");
            var fun = ce.Function == "printd" ? "printf" : ce.Function;
            yield return new AsmLine("call", fun, null);
            if (ce.Function == "printf" || ce.Function == "printd" || ce.Function == "VirtualAlloc")
            {
                yield return new AsmLine("add","rsp", "40");
                ce.Context.Pop(40);
            }
            yield return new AsmLine("mov",NewReg(), "rax");
        }

        public IEnumerable<AsmLine> Translate(Expression expr)
        {
            switch (expr)
            {
                case BinExpression be:
                {
                    foreach (var op in Assigners.Contains(be.Operation) ? TranslateAssign(be) : Translate(be))
                        yield return op;
                    break;
                }
                case UnExpression ue:
                {
                    switch (ue.Operation)
                    {
                        case "&" when ue.Operand is VarExpression ve:
                            yield return new AsmLine("mov", NewReg(), $"rsp");
                            yield return new AsmLine("add", $"#{_regUsed}#", $"{ve.Context.GetOffset(ve.Variable)}");
                            break;
                        case "*":
                        {
                            foreach (var op in Translate(ue.Operand))
                                yield return op;
                            var regcode = $"[#{_regUsed}#]";
                            yield return new AsmLine("mov", NewReg(), regcode);
                            break;
                        }
                        default:
                            throw new Exception($"Wrong usage of {ue.Operation}");
                    }

                    break;
                }
                case CallExpression ce:
                {
                    foreach (var op in Translate(ce))
                        yield return op;
                    break;
                }
                case ConstExpression constE:
                {
                    yield return new AsmLine("mov", NewReg(), constE.Const);
                    break;
                }
                case VarExpression varE:
                {
                    if (varE.Context.Arrays.Contains(varE.Variable))
                    {
                        yield return new AsmLine("mov", NewReg(), $"rsp");
                        yield return new AsmLine("add", $"#{_regUsed}#", $"{varE.Context.GetOffset(varE.Variable)}");
                    }
                    else
                        yield return new AsmLine("mov", NewReg(), $"{varE}");
                    break;
                }
            }
        }

        public static HashSet<string> Spoils(AsmLine instr)
        {
            if (instr.instr == "call")
                return new HashSet<string>{"rcx","rdx", "r8", "r9", "rax", "r10", "r11"};
            if (instr.left[0] != '[')
                return new HashSet<string>{instr.left};
            return new HashSet<string>();
        }

        private static Dictionary<string, string> x64TOx8 = new Dictionary<string, string>
        {
            ["rax"] = "al",
            ["rbx"] = "bl",
            ["rdx"] = "dl",
            ["rcx"] = "cl",
            ["rsp"] = "spl",
            ["rbp"] = "bpl",
            ["rsi"] = "sil",
            ["rdi"] = "dil",
            ["r8"] = "r8b",
            ["r9"] = "r9b",
            ["r10"] = "r10b",
            ["r11"] = "r11b",
            ["r12"] = "r12b",
            ["r13"] = "r12b",
            ["r14"] = "r14b",
            ["r15"] = "r15b",
        };
        
        public static List<AsmLine> Inline(List<AsmLine> code)
        {
            
            for (var i = 0;i<code.Count;)
            {
                if (code[i].instr != "mov" || char.IsDigit(code[i].right[0]) && code[i].right.Length > 8)
                {
                    i++;
                    continue;
                }

                var flag = false;
                for (var j = i + 1; j < code.Count; j++)
                {
                    if (Spoils(code[j]).Contains(code[i].right) || Spoils(code[j]).Contains(code[i].left))
                        break;

                    if (code[j].instr == "call")
                        continue;
                    
                    if (code[i].right[0] == '[' && code[j].left[0] == '[')
                        break;
                    
                    if (code[i].left != code[j].right)
                        continue;

                    if (code[j].instr.StartsWith("set"))
                        break;
                    
                    flag = true;
                    Console.WriteLine($"{code[i]} |-> {code[j]}");
                    code[j].right = code[i].right;
                    code.RemoveAt(i);
                    break;
                }

                if (!flag)
                    i++;
            }
            return code;
        }
        
        public static List<string> Regs = new List<string>
        {
            "r10", "r11", "rcx", "rdx", "r8", "r9", "r12", "r13", "rbx", "r12", "r13", "r14", "r15"
        };

        public HashSet<string> usedRegs = new HashSet<string>();

        public string result;
        
        public static bool Intersects((int, int) f, (string, int, int) s)
        {
            return !(f.Item2 < s.Item2 || s.Item3 < f.Item1);
        }

        public List<AsmLine> PutRegiters(List<AsmLine> code)
        {
            var regs = code.SelectMany(c => new[] {c.left, c.right}).Where(s => s != null && s[0] == '#').Distinct();

            var taken = new Dictionary<string, (string, int, int)>();
            
            foreach (var r in regs)
            {
                var first = code.FindIndex(x => x.left == r);
                var last = code.FindLastIndex(x => x.right != null && (x.right == r || x.right == $"[{r}]") || x.left == r || x.left == $"[{r}]");
                Console.WriteLine($"{r}: {first} -> {last}");
                var replacedSuccessfully = false;
                foreach (var reg in Regs)
                {
                    var cFlag = false;
                    for (var i = first; i <= last; i++)
                    {
                        if (Spoils(code[i]).Contains(reg))
                        {
                            cFlag = true;
                            break;
                        }
                    }
                    if (taken.Count(kv => kv.Value.Item1 == reg && Intersects((first, last), kv.Value)) != 0)
                        continue;
                    if (cFlag)
                        continue;
                    taken[r] = (reg, first, last);
                    usedRegs.Add(reg);
                    var newReg = reg;
                    if (code[first].instr.StartsWith("set"))
                        newReg = x64TOx8[reg];
                    Console.WriteLine($"{r}-> {newReg}");
                    foreach (var instr in code)
                    {
                        instr.left = instr.instr.StartsWith("set") 
                            ? instr.left.Replace(r, newReg) 
                            : instr.left.Replace(r, reg);
                        
                        if (instr.right != null)
                            instr.right = instr.right.Replace(r, reg);
                    }
                    replacedSuccessfully = true;
                    break;
                    //Console.WriteLine($"{r}: {first} -> {last}");
                }
                if (!replacedSuccessfully)
                    throw new Exception("Not enough registers");
            }

            result = code[^1].instr == "call" ? "rax" : code[^1].left;
            
            return code;
        }
    }
}