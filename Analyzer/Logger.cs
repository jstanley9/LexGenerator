using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LexBatch.Analyzer
{
    public class Logger : ILogger
    {
        public Level LogLevel { get; set; } = Level.Warning;
        private readonly StreamWriter logFile = null;

        public string LogFileTitle { get; private set; } 

        private Logger(string logFileTitle)
        {
            logFile = new StreamWriter(logFileTitle);
        }

        public static ILogger GetLogger(string programName)
        {
            string logFileTitle = Path.Combine(Path.GetTempPath(), $"{programName}.log");
            return new Logger(logFileTitle);
        }

        public void Log(Level logLevel, string message)
        {
            if (((int)logLevel & (int)LogLevel) != 0)
            {
                string timedMessage = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss.fffffK}: {message}";
                logFile.WriteLine(timedMessage);
            }
        }

        public void Log(Level logLevel, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void StopLogging()
        {
            logFile.Close();
        }
    }
}
