using LexBatch.Analyzer;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace LexBatch.LexInterfaces
{
    public interface ILogger
    {
        Level LogLevel { get; set; }
        string LogFileTitle { get; }

        void StopLogging();
        void Log(Level logLevel, string message);
        void Log(Level logLevel, string message, Exception exception);

    }
}
