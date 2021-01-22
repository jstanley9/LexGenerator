using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class ByNFAStateNumber : IComparer<NFA>
    {
        public int Compare(NFA state1, NFA state2)
        {
            if (state1.StateNumber > state2.StateNumber)
            {
                return 1;
            }
            if (state1.StateNumber < state2.StateNumber)
            {
                return -1;
            }
            return 0;
        }
    }
}
