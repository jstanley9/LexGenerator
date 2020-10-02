using System.ComponentModel;

namespace LexBatch.Analyzer
{
    public enum ELoggingOptions
    {
        [Description("Not Logging")]
        NoLogging,
        [Description("Log Option Print statistics")]
        PrintStatistics,
        [Description("Log Option: Print internal diagnostics and statistics")]
        PrintInternalDiagnostics
    }
}
