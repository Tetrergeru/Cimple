using System.Collections.Generic;
using System.Linq;

namespace Cimple
{
    public class Token
    {
        public string File { get; }

        public int Line { get; }
        
        public int Start { get; }
        
        public int Length => Text.Length;

        public string Text { get; }

        public TokenType TType { get; }

        public Token(string text, string file = "", int line = 0, int start = 0) 
        {
            File = file;
            Text = text;
            Line = line;
            Start = start;
            TType = ParseTokenType(text);
        }

        public override string ToString()
            => $"[file: {File}, Line: {Line}, Text: \"{Text}\"]";

        public static HashSet<string> Parenthesis = new HashSet<string> {
            "[", "]",
            "(", ")",
            "{", "}"
        };
        
        public static HashSet<string> Operators = new HashSet<string> {
            "+", "-", "*", "/", "%", "=",
            "+=", "-=", "*=", "/=",
            ">>", "<<", "!=", "==", "<=", ">=",
            "|", "&", "||", "&&"
        };        

        public static TokenType ParseTokenType(string token)
        {
            if (token.Length == 0)
                return TokenType.None;
            
            if (char.IsDigit(token[0]))
                return TokenType.Const;
            
            if (token.All(c => char.IsLetter(c) || char.IsDigit(c) || c == '_'))
                return TokenType.Name;
            
            if (token.Length > 1 && token[0] == 'u' && token.Skip(1).All(char.IsDigit))
                return TokenType.Type;

            if (Parenthesis.Contains(token))
                return TokenType.Bracket;

            if (Operators.Contains(token))
                return TokenType.Operator;

            return token switch
            {
                "for" => TokenType.For,
                "if" => TokenType.If,
                _ => TokenType.None
            };
        }
    }
}