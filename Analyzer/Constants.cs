namespace LexBatch.Analyzer
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

        public const string CLOSE_CURLY_BRACE = "}";
        public const string ESCAPE = "\\";
        public const string NEW_LINE = "\n";
        public const string OPEN_CURLY_BRACE = "{";
        public const string QUOTE = "\"";
        public const string SPACE = " ";
    }
}
