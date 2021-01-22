using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    internal class LeXDFAState
    {
        private uint Group { get; set; } = 8;
        private uint Mark { get; set; } = 1;
        private string Accept { get; set; } = null;
        private EItemAnchor Anchor { get; set; } = EItemAnchor.NoAnchor;

        private List<NFA> NFAStates;
    }
}
