using LexBatch.Analyzer;
using LexBatch.LexInterfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace LexBatch
{
    class LexBatch
    {
        private readonly ILexConfigure config = null;

        private LexBatch()
        {
            config = LexConfiguration.GetInstance();
        }

        private void BuildLexicalAnalyzer()
        {
        //    Analyzable lex = new LeXAnalyzer(config);
        }

        private bool ProcessArgs(string[] args)
        {
            ProcessArguments ArgParser = new ProcessArguments(args, config);
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
                    Lex.config.InitLogging();
                    Lex.BuildLexicalAnalyzer();
                }
            }
            finally
            {
                Lex.config.CloseLogging();
            }
        }
    }
}
