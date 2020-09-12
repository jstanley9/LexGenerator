using LexGenerator.LexInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexGenerator.Analyzer
{
    public class LexConfigure : ILexConfigure
    {
        private static ILexConfigure config = null;
        public int ErrorCount { get; private set; }
        public string InputFileTitle { get; set; } = "C:/Temp/LexDef.lex";
        public string LogFileTitle { get; set; } = "C:/Temp/LexLog.log";
        public string OutputFileTitle { get; set; } = "C:/Temp/lexyy.cs";
        public LoggingOptions Verbose { get; private set; } = LoggingOptions.NoLogging;
        private Logger logger = null;

        private LexConfigure()
        {

        }
        public static ILexConfigure GetInstance()
        {
            if (config == null)
            {
                config = new LexConfigure();
            }
            return config;
        }

        public void Error(int LineNumber, string text, ErrorCodes error)
        {
            string err = $"Error @{LineNumber}: {ILexConfigure.EnumToString(error)}";
            Console.WriteLine(err);
            Console.WriteLine(text);
            logSevere(err);
            logSevere(text);
            ErrorCount++;
        }


    }
}
