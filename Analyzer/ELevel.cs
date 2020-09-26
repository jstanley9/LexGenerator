using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LexBatch.Analyzer
{
    public enum ELevel
    {
        Off = 0b_0000_0000,    // No logging
        Severe = 0b_0000_0001,
        Warning = 0b_0000_0011,
        Info = 0b_0000_0111,
        Config = 0b_0000_1111,
        Fine = 0b_0001_1111,
        Finer = 0b_0011_1111,
        Finest = 0b_0111_1111,
        All = 0b_1111_1111
    }
}
