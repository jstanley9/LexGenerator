using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LexBatch.Analyzer
{
    public static class LexConfiguration
    {
        private static readonly Stack<string> _callStack = new Stack<string>(32);
        public static int ErrorCount { get; private set; }
        public static string InputFileTitle { get; set; } = "C:/Temp/LexDef.lex";
        public static string LogFileTitle { get; set; } = "LexLog";
        public static string OutputFileTitle { get; set; } = "C:/Temp/lexyy.cs";
        public static ELoggingOptions Verbose { get; set; } = ELoggingOptions.NoLogging;
        private static ILogger logger = null;


        public static void CloseLogging() => logger.StopLogging();

        public static void Error(int lineNumber, string text, EErrorCodes error)
        {
            string err = $"Error @{lineNumber}: {GetEnumDescription(error)}";
            Console.WriteLine(err);
            Console.WriteLine(text);
            LogSevere(err);
            LogSevere(text);
            ErrorCount++;
        }

        public static string GetEnumDescription(Enum enumValue)
        {
            var memberInfo = GetEnumMemberInfo(enumValue);
            var attributes = memberInfo[0].GetCustomAttributes<DescriptionAttribute>();// (typeof(DescriptionAttribute), false);
            var description = (DescriptionAttribute)attributes.FirstOrDefault();
            return description == null ? String.Empty
                                       : description.Description;
        }

        private static MemberInfo[] GetEnumMemberInfo(Enum enumValue)
        {
            var type = enumValue.GetType();
            var typeInfo = type.GetTypeInfo();
            return typeInfo.GetMember(enumValue.ToString());
        }

        public static string GetEnumName(Enum enumValue) => GetEnumMemberInfo(enumValue)[0].Name;

        public static void InitLogging()
        {
            logger = Logger.GetLogger(LogFileTitle);
            logger.LogLevel = Verbose switch
            {
                ELoggingOptions.NoLogging => ELevel.Warning,
                ELoggingOptions.PrintStatistics => ELevel.Fine,
                ELoggingOptions.PrintInternalDiagnostics => ELevel.All,
                _ => ELevel.Warning,
            };
        }

        private static void LogLevel(ELevel requestedLevel, string message) => logger.Log(requestedLevel, message);

        public static void LogFine(string message) => LogLevel(ELevel.Fine, message);

        public static void LogInfo(string message) => LogLevel(ELevel.Info, message);

        public static void LogSevere(string message) => LogLevel(ELevel.Severe, message);

        public static void LogEnter(string procName)
        {
            _callStack.Push(procName);
            LogInfo($"Enter {LogCall(_callStack.Count, procName)}");
        }

        public static void LogExit(string procName = "")
        {
            int count = _callStack.Count;
            string message = _callStack.Pop();
            message = procName.Length > 0 ? procName : message;
            LogInfo($"Exit {LogCall(count, message)}");
        }

        private static string LogCall(int count, string procName) => $"{count}: {procName}";

        public static void ParseError(EErrorCodes errorMessage)
        {
            ErrorCount++;
            LogSevere($"{Constants.Error}{errorMessage}");
        }
    }
}
