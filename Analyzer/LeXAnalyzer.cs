using LexBatch.LexInterfaces;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class LeXAnalyzer : IAnalyzable
    {
        private readonly List<ISourceLine> codeConstants = new List<ISourceLine>();
        private readonly List<NFA> NFAList = new List<NFA>(4096);

        public void Generate()
        {
            NFA startingState = ProcessInput();
            ConvertNFAtoDFA(startingState);
            GenerateLexicalParser();
        }

        private NFA ProcessInput()
        {
            IInputProcessor thompson = new LexInput(codeConstants, NFAList);
            return thompson.GenerateNFA();
        }

        private void ConvertNFAtoDFA(NFA startingState)
        {
            LeXDFA dfa = new LeXDFA(NFAList);
            List<Accept> acceptList = new List<Accept>();
            int dfaCount = dfa.DFA(startingState, acceptList);
            List<DFAState> DStates = dfa.DStates;
        }

        private void GenerateLexicalParser()
        {
            //ProduceLexicalParser();
        }

    }
}
