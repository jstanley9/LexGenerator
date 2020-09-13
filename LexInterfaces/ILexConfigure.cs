using LexBatch.Analyzer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace LexBatch.LexInterfaces
{
    public interface ILexConfigure
    {
        string InputFileTitle { get; set; }
        string LogFileTitle { get; set; }
        string OutputFileTitle { get; set; }
        LoggingOptions Verbose { get; set; }

        static string EnumToString(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }

        void CloseLogging();
        void Error(int LineNumber, string text, ErrorCodes error);
        void InitLogging();
        void LogFine(string message);
        void LogInfo(string message);
        void LogSevere(string message);
        void ParseError(ErrorCodes errorMessage);

        
    }
}
