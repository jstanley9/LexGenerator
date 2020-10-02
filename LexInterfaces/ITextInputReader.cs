namespace LexBatch.LexInterfaces
{
    interface ITextInputReader
    {
        bool Close();
        string GetNextLine();
    }
}
