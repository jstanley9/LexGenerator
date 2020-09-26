using System;
using System.Collections.Generic;
using System.Text;

namespace LexBatch.LexInterfaces
{
    public interface IRuleInput
    {
        string CurrentAcceptActions { get; }
        IToken Advance(IToken lastToken);
    }
}
