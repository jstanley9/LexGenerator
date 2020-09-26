using LexBatch.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace LexBatch.LexInterfaces
{
    public interface IToken
    {
        int LineNumber { get; set; }
        string Token { get; set; }
        ETokenType TokenType { get; set; }

        bool IsTokenType(ETokenType tokenType);
        string ToString();
    }
}
