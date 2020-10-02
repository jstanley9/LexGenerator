namespace LexBatch.LexInterfaces
{
    public interface IRuleInput
    {
        string CurrentAcceptActions { get; }
        IToken Advance(IToken lastToken);
    }
}
