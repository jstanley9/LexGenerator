namespace LexBatch.LexInterfaces
{
    internal interface ITextInputReader
    {
        bool Close();
        string GetNextLine();
    }
}
