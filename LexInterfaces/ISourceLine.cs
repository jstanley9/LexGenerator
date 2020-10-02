namespace LexBatch.LexInterfaces
{
    public interface ISourceLine
    {
        string Line { get; }
        int LineNumber { get; }
        bool IsBlankLine();
        string ToString();
    }
}
