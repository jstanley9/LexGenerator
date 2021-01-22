using LexBatch.LexInterfaces;

namespace LexBatch.Analyzer
{
    public class LeXToken : IToken
    {
        public int LineNumber { get; set; }
        public string Token { get; set; }
        public ETokenType TokenType { get; set; }

        public LeXToken(int lineNumber, string token, ETokenType type) => (LineNumber, Token, TokenType) = (lineNumber, token, type);

        public bool HasCharacter(char Match_Character) => Token.Contains(Match_Character);

        public bool IsTokenType(ETokenType tokenType) => this.TokenType == tokenType;

        public override string ToString() => $"{LineNumber}: '{Token}' {LexConfiguration.GetEnumName(TokenType)}";
    }
}
