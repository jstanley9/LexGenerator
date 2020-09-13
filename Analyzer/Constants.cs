using System;
using System.Collections.Generic;
using System.Text;

namespace LexGenerator.Analyzer
{
    static public class Constants
    {
        public const string PrefixStart = "%{";
        public const string PrefixEnd = "%}";
        public const string RulesBoundary = "%%";

        public const string ArgPrefix = "-";
        public const string ArgHelp = "?";
        public const string ArgHelpCommand = ArgPrefix + ArgHelp;
        public const string ArgInput = "input";
        public const string ArgLog = "log";
        public const string ArgOutput = "output";
        public const string Error = "Error: ";
    }
}
