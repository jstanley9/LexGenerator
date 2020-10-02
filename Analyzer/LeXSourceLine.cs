using LexBatch.LexInterfaces;

namespace LexBatch
{
    class LeXSourceLine : ISourceLine
    {
        public string Line { get; private set; }
        public int LineNumber { get; private set; }

        public LeXSourceLine(int lineNbr, string sourceLine)
        {
            LineNumber = lineNbr;
            Line = sourceLine;
        }
        public bool IsBlankLine() => Line.Trim().Length == 0;

        public override string ToString() => $"{LineNumber}: {Line}";
    }
}
