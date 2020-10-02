using LexBatch.LexInterfaces;
using System;
using System.IO;

namespace LexBatch.Analyzer
{
    class LeXLineInput : ILineInput
    {
        public bool FileIsOpen { get; private set; }
        private ITextInputReader Input { get; set; }
        public string Line { get; private set; }
        public int LineNumber { get; private set; }

        public LeXLineInput()
        {
            OpenTextFile(LexConfiguration.InputFileTitle);
            LineNumber = 0;
            if (FileIsOpen)
            {
                ReadAhead();
            }
        }

        public void Close()
        {
            try
            {
                Input.Close();
            }
            catch (IOException closeExcept)
            {
                LexConfiguration.LogSevere($"File error: Closing {LexConfiguration.InputFileTitle} {closeExcept.Message}");
            }
        }

        public bool HasNext() => Line != null && Line != String.Empty;

        public bool NextLineStartsWith(string startsWith) => HasNext() && Line.StartsWith(startsWith);

        public bool NextLineStartsWithWhiteSpace() => HasNext() && Char.IsWhiteSpace(Line, 0);

        private void OpenTextFile(string title)
        {
            FileIsOpen = false;
            if (File.Exists(title))
            {
                FileAttributes attr = File.GetAttributes(title);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    LexConfiguration.LogSevere($"File error: {title} is not a file");
                }
                else
                {
                    try
                    {
                        Input = new LeXTextInputReader(title);
                        FileIsOpen = true;
                    }
                    catch (FileNotFoundException notFound)
                    {
                        LexConfiguration.LogSevere($"File error: {title} {notFound.Message}");
                    }

                }
            }
            else
            {
                LexConfiguration.LogSevere($"File error: {title} not found");
            }
        }

        private void ReadAhead()
        {
            try
            {
                Line = Input.GetNextLine();
            }
            catch (IOException ioError)
            {
                LexConfiguration.LogSevere($"File error: reading from {LexConfiguration.InputFileTitle} {ioError.Message}");
                Line = String.Empty;
            }
        }

        public ISourceLine ReadLine()
        {
            string lineRead = Line;
            int thisLineNumber = ++LineNumber;

            if (HasNext())
            {
                ReadAhead();
            }
            else
            {
                thisLineNumber = 0;
            }
            ISourceLine resultLine = new LeXSourceLine(thisLineNumber, lineRead);
            LexConfiguration.LogFine($"Input: {resultLine}");
            return resultLine;
        }

        public override string ToString() => $"{LineNumber}: {Line}";
    }
}
