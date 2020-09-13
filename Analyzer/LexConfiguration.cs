using LexBatch.LexInterfaces;
using LexGenerator.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexBatch.Analyzer
{
    public class LexConfiguration : ILexConfigure
    {
        private static ILexConfigure config = null;
        public int ErrorCount { get; private set; }
        public string InputFileTitle { get; set; } = "C:/Temp/LexDef.lex";
        public string LogFileTitle { get; set; } = "LexLog";
        public string OutputFileTitle { get; set; } = "C:/Temp/lexyy.cs";
        public LoggingOptions Verbose { get; set; } = LoggingOptions.NoLogging;
        private ILogger logger = null;

        private LexConfiguration()
        {

        }

        public static ILexConfigure GetInstance()
        {
            if (config == null)
            {
                config = new LexConfiguration();
            }
            return config;
        }

        public void CloseLogging()
        {
            logger.StopLogging();
        }

        public void Error(int lineNumber, string text, ErrorCodes error)
        {
            string err = $"Error @{lineNumber}: {ILexConfigure.EnumToString(error)}";
            Console.WriteLine(err);
            Console.WriteLine(text);
            LogSevere(err);
            LogSevere(text);
            ErrorCount++;
        }

        public void InitLogging()
        {
            logger = Logger.GetLogger(LogFileTitle);
            logger.LogLevel = Verbose switch
            {
                LoggingOptions.NoLogging => Level.Warning,
                LoggingOptions.PrintStatistics => Level.Fine,
                LoggingOptions.PrintInternalDiagnostics => Level.All,
                _ => Level.Warning,
            };
        }

        private void LogLevel(Level requestedLevel, string message)
        {
            logger.Log(requestedLevel, message);
        }

        public void LogFine(string message)
        {
            LogLevel(Level.Fine, message);
        }

        public void LogInfo(string message)
        {
            LogLevel(Level.Info, message);
        }

        public void LogSevere(string message)
        {
            LogLevel(Level.Severe, message);
        }

        public void ParseError(ErrorCodes errorMessage)
        {
            ErrorCount++;
            LogSevere($"{Constants.Error}{errorMessage}");
        }
    }
}
