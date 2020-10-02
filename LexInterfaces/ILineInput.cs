namespace LexBatch.LexInterfaces
{
    public interface ILineInput
    {
        bool FileIsOpen { get; }
        int LineNumber { get; }

        void Close();
        bool HasNext();
        bool NextLineStartsWith(string startsWith);
        bool NextLineStartsWithWhiteSpace();
        ISourceLine ReadLine();
        string ToString();
    }
}
