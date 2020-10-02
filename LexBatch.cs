using LexBatch.Analyzer;
using LexBatch.LexInterfaces;

namespace LexBatch
{
    class LexBatch
    {
        private void BuildLexicalAnalyzer()
        {
            IAnalyzable lex = new LeXAnalyzer();
            lex.Generate();
        }

        private bool ProcessArgs(string[] args)
        {
            ProcessArguments ArgParser = new ProcessArguments(args);
            bool result = ArgParser.ParseArguments();
            return result;
        }

        static void Main(string[] args)
        {
            LexBatch Lex = new LexBatch();
            try
            {
                if (Lex.ProcessArgs(args))
                {
                    LexConfiguration.InitLogging();
                    Lex.BuildLexicalAnalyzer();
                }
            }
            finally
            {
                LexConfiguration.CloseLogging();
            }
        }
    }
}
