using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace LexBatch.Analyzer
{
    public class LeXAnalyzer : IAnalyzable
    {
        private readonly List<ISourceLine> codeConstants = new List<ISourceLine>();
        private readonly List<NFA> NFAList = new List<NFA>(4096);

        public void Generate()
        {
            int startingStateIndex = ProcessInput();
            ConvertNFAtoDFA(startingStateIndex);
            GenerateLexicalParser();
        }

        private int ProcessInput()
        {
            IInputProcessor thompson = new LexInput(codeConstants, NFAList);
            return thompson.GenerateNFA();
        }

        private void ConvertNFAtoDFA(int startingStateIndex)
        {
            //ReduceNFAToDFA();
        }

        private void GenerateLexicalParser()
        {
            //ProduceLexicalParser();
        }

    }
}
