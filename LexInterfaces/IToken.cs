using LexBatch.Analyzer;

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
