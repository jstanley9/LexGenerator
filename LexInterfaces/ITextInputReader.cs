using System;
using System.Collections.Generic;
using System.Text;

namespace LexBatch.LexInterfaces
{
    interface ITextInputReader
    {
        bool Close();
        string GetNextLine();
    }
}
