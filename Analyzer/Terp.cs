using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    internal class Terp
    {
        private readonly IComparer<NFA> NFAStateComparator;

        public Terp(IComparer<NFA> StateComparator = null)
        {
            NFAStateComparator = StateComparator ?? new ByNFAStateNumber();
        }

        private void CheckNext(NFA CheckNFA, DFAState CurrentState, Stack<NFA> Stack)
        {
            if (CheckNFA != null)
            {
                if (CurrentState.IsMember(CheckNFA))
                {
                    CurrentState.AddNFA(CheckNFA);
                    Stack.Push(CheckNFA);
                }
            }
        }

        public DFAState E_Closure(DFAState Input)
        {
            if (Input == null)
            {
                return Input;
            }

            Stack<NFA> stack = new Stack<NFA>();

            foreach (NFA nextNFA in Input)
            {
                stack.Push(nextNFA);
            }

            NFA currentNFA;
            int lowest_accepting_state = int.MaxValue;
            DFAState resultDFAState = new DFAState();

            while (stack.Count > 0)
            {
                currentNFA = stack.Pop();
                if (currentNFA.HasAccept() && currentNFA.StateNumber < lowest_accepting_state)
                {
                    lowest_accepting_state = currentNFA.StateNumber;
                    resultDFAState.Accept = currentNFA.Accept;
                    resultDFAState.Anchor = currentNFA.Anchor;
                }

                if (currentNFA.Edge == EEdgeType.Epsilon)
                {
                    CheckNext(currentNFA.Next, resultDFAState, stack);
                    CheckNext(currentNFA.Next2, resultDFAState, stack);
                    if (!resultDFAState.IsMember(currentNFA))
                    {
                        resultDFAState.AddNFA(currentNFA);
                    }
                }
            }
            return resultDFAState;
        }

        public DFAState Move(DFAState Inp_Set, char Transition_Char)
        {
            /* Return a set that contains all NFA states that can be reached by making transitions on "Transition_Char" from any
             * NFA state in "Inp_Set". returns NULL if there are no such transitions.
             * The "Inp_Set" is not modified
             */

            DFAState OutSet = null;

            foreach (NFA State in Inp_Set)
            {
                if (State.HasTransition(Transition_Char))
                {
                    if (OutSet == null)
                    {
                        OutSet = new DFAState(NFAStateComparator);
                    }
                    OutSet.AddNFA(State);
                }
            }
            return OutSet;
        }
    }
}
