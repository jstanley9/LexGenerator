using LexBatch.Analyzer;

namespace LexBatch.LexInterfaces
{
    public interface IToken
    {
        int LineNumber { get; set; }
        string Token { get; set; }
        ETokenType TokenType { get; set; }

        bool HasCharacter(char Match_Character);
        bool IsTokenType(ETokenType tokenType);
        string ToString();
    }
}
