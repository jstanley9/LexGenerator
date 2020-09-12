using System;

namespace LexGenerator
{
    class LexBatch
    {
        static void Main(string[] args)
        {
            LexBatch Lex = new LexBatch();
            if (Lex.ProcessArgs(args))
            {
                Lex.BuildLexicalAnalyzer();
            }
        }
    }
}
