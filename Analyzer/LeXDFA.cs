using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class LeXDFA
    {
        private static char MAX_CHARS = (char)128;
        private static int FAILURE = -1;
        private int _nbrStates = 0;
        private int _last_Marked = 0;        // Most-recently marked DFA state in DTran
        private readonly List<NFA> NFAList;
        private readonly List<DFAState> DStates;

        public List<int> DTran { get; private set; }

        private readonly IComparer<NFA> NFAStateComparator = new ByNFAStateNumber();

        public LeXDFA(List<NFA> NFAStates)
        {
            NFAList = NFAStates;
            int halfStates = NFAList.Count / 2;
            DStates = new List<DFAState>(halfStates);
            DTran = new List<int>(halfStates);
        }

        public int DFA(NFA startingState, List<Accept> acceptList)
        {
            _nbrStates = 0;
            DStates.Clear();
            DTran.Clear();
            _last_Marked = 0;

            LexConfiguration.LogFine("Making DFA");
            Make_Dtran(startingState);              // Convert the NFA to a DFA

            acceptList.Clear();

            for (int i = _nbrStates; --i >= 0;)
            {
                DFAState localDFA = DStates[i];
                acceptList.Add(new Accept(localDFA.Accept, localDFA.Anchor));
            }

            if (LexConfiguration.Verbose == ELoggingOptions.PrintInternalDiagnostics)
            {
                LexConfiguration.LogFine($"{_nbrStates} DFA states out of {NFAList.Count} NFA states");
                LexConfiguration.LogFine($"{_nbrStates * MAX_CHARS + _nbrStates} bytes required for uncompressed tables");
                LexConfiguration.LogFine("The un-minimized DFA looks like this\n");
            }

            return _nbrStates;
        }

        private DFAState Get_Unmarked()
        {
            for (; _last_Marked < DStates.Count; ++_last_Marked)
            {
                DFAState DFA = DStates[_last_Marked];
                if (DFA.Mark > 0)
                {
                    LexConfiguration.LogFine($"Working on DFA state {DFA.StateNumber}[{_last_Marked}] states ({DFA.StateList()})");
                    return DFA;
                }
            }
            return null;
        }

        private DFAState In_DStates(DFAState SearchState)
        {
            /* If there is a set in DStatesthat is identical to DFAState return the state, else return null */
            foreach (DFAState state in DStates)
            {
                if (SearchState.Is_Equivalent(state))
                {
                    return state;
                }
            }
            return null;
        }

        private void Make_Dtran(NFA startingState)
        {
            DFAState nextState;          // GoTo DFA state for current char
            string isAccept;        // Current DFA state is accepting    
            EItemAnchor anchor;

            /* Initially DStates contains a single, unmarked, start state formed by taking the epsilon closure of the
             * NFA start state. So, DStates[0] (and DTran[0]) is the DFA start state.
             */
            DFAState NFA_Set = new DFAState(NFAStateComparator); // NFA states defining the next DFA state
            NFA_Set.AddNFA(startingState);
            Terp NFA_To_DFA = new Terp(NFAStateComparator);

            _nbrStates = 1;
            DFAState current = NFA_To_DFA.E_Closure(NFA_Set);
            current.Mark = 0;
            DStates.Add(current);

            while ((current = Get_Unmarked()) != null)
            {
                current.Mark = 1;

                for (char c = MAX_CHARS; --c >= 0;) // For now we will deal only with the traditional ASCII character set
                {
                    NFA_Set = NFA_To_DFA.Move(current, c);
                    if (NFA_Set != null)
                    {
                        NFA_Set = NFA_To_DFA.E_Closure(NFA_Set);
                    }
                    if (NFA_Set == null)
                    {
                        nextState = null; // FAILURE;
                    }
                    else if ((nextState = In_DStates(NFA_Set)) != null)
                    {
                        NFA_Set = null;
                    }
                    else
                    {
                        nextState = add_to_dstates(NFA_Set, isAccept, anchor);
                    }
                    DTran[current - DStates][c] = nextState;
                }
            }

        }

    }
}
