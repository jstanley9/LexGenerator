using System.Collections;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class DFAState : IEnumerable<NFA>
    {
        private static uint _state_Number = 0;
        public uint StateNumber { get; private set; }
        public uint Group { get; set; } = 8;       // Group id, used by minimize()
        public uint Mark { get; set; } = 1;         // Mark used by Make_Dtran  in LeXDFA
        public string Accept { get; set; }         // Accept action if an accept state
        public EItemAnchor Anchor { get; set; }    // Anchor point if an accept state
        private SortedSet<NFA> _NFA_Set;           // Set of NFA states represented by this DFA state

        public DFAState(IComparer<NFA> comparator = null)
        {
            _NFA_Set = new SortedSet<NFA>(comparator ?? new ByNFAStateNumber());
            StateNumber = _state_Number++;
        }

        public void AddNFA(NFA NFA_State)
        {
            if (!(_NFA_Set.Contains(NFA_State)))
            {
                _NFA_Set.Add(NFA_State);
                if (Accept.Length == 0 && NFA_State.Accept.Length != 0)
                {
                    Accept = NFA_State.Accept;
                }
                if (NFA_State.IsAnchored() && (Anchor == EItemAnchor.NoAnchor))
                {
                    Anchor = NFA_State.Anchor;
                }
            }
        }

        public bool Is_Equivalent(DFAState State) => (Accept.Equals(State.Accept) && _NFA_Set.Equals(State._NFA_Set));

        public bool IsMember(NFA checkNFA)
        {
            return _NFA_Set.Contains(checkNFA);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _NFA_Set.GetEnumerator();
        }

        public IEnumerator<NFA> GetEnumerator()
        {
            return _NFA_Set.GetEnumerator();
        }
    }
}
