using LexBatch.Analyzer;
using System.Collections.Generic;

namespace LexBatch.LexInterfaces
{
    internal interface IInterpretNFA
    {
        List<NFA> E_Closure(List<NFA> inputNFAStates, string accept, EItemAnchor eItemAnchor);
        List<NFA> Move(List<NFA> inputNFAStates, string transition);
    }
}
