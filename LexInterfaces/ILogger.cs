using LexBatch.Analyzer;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace LexBatch.LexInterfaces
{
    public interface ILogger
    {
        ELevel LogLevel { get; set; }
        string LogFileTitle { get; }

        void StopLogging();
        void Log(ELevel logLevel, string message);
        void Log(ELevel logLevel, string message, Exception exception);

    }
}
