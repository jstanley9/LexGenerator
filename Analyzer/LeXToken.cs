using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexBatch.Analyzer
{
    public class LeXToken : IToken
    {
        public int LineNumber { get; set; }
        public string Token { get; set; }
        public ETokenType TokenType { get; set; }

        public LeXToken(int lineNumber, string token, ETokenType type)
        {
            this.LineNumber = lineNumber;
            this.Token = token;
            this.TokenType = type;
        }

        public bool IsTokenType(ETokenType tokenType)
        {
            return (this.TokenType == tokenType);
        }

        public override string ToString() => $"{LineNumber}: '{Token}' {LexConfiguration.GetEnumDescription(TokenType)}";
    }
}
