using LexBatch.LexInterfaces;
using System.IO;

namespace LexBatch.Analyzer
{
    class LeXTextInputReader : ITextInputReader
    {
        private readonly StreamReader Input;

        public LeXTextInputReader(string title)
        {
            Input = File.OpenText(title);
        }

        public bool Close()
        {
            Input.Close();
            return true;
        }

        public string GetNextLine()
        {
            return Input.ReadLine();
        }
    }
}
