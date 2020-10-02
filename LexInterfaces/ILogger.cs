using LexBatch.Analyzer;
using System;

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
