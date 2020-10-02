using System;

namespace LexBatch.Analyzer
{
    public class ProcessArguments
    {
        private int ArgIndex { get; set; } = 0;
        private readonly string[] args;

        public ProcessArguments(string[] args)
        {
            this.args = args;
        }

        public bool ParseArguments()
        {
            if (args == null || args.Length <= 0 || args[0].StartsWith(Constants.ArgHelp) || args[0].StartsWith(Constants.ArgHelpCommand))
            {
                ShowHelp(String.Empty);
                return false;
            }

            ArgIndex = 0;

            while (ArgIndex < args.Length)
            {
                string arg = args[ArgIndex];
                if (arg.StartsWith(Constants.ArgPrefix))
                {
                    ArgIndex++;
                    switch (ParseArgument(arg))
                    {
                        case EParseArgResult.ArgOK:
                            break;
                        case EParseArgResult.ArgPlusOneOK:
                            ArgIndex++;
                            break;
                        case EParseArgResult.ArgIncorrect:
                            return false;
                        default:
                            return false;
                    }
                }
            }

            return true;
        }

        private static void ShowHelp(string errorArg)
        {
            Console.WriteLine();
            if (errorArg.Length > 0)
            {
                Console.WriteLine($"{Constants.Error}{errorArg}");
            }

            Console.WriteLine("Usage is: LeX [options] -i[nput] file [-0o[utput] file] [-log file]");
            Console.WriteLine("      or: Lex ? for help");
            Console.WriteLine();
            Console.WriteLine("-v (v)erbose mode, Log statistics");
            Console.WriteLine("-V (V)erbose mode, Log statistics and internal diagnostics");
        }

        private string GetNextArg()
        {
            string nextArg = Constants.ArgPrefix;
            if (ArgIndex < args.Length)
            {
                nextArg = args[ArgIndex];
            }
            return nextArg;
        }

        private EParseArgResult ParseArgument(string arg)
        {
            bool continueEvaluation = true;
            EParseArgResult result = EParseArgResult.ArgOK;
            for (int pos = 1; (continueEvaluation && pos < arg.Length); pos++)
            {
                string charAtPos = arg.Substring(pos, 1);
                switch (charAtPos)
                {
                    case "v":
                        LexConfiguration.Verbose = ELoggingOptions.PrintStatistics;
                        break;
                    case "V":
                        LexConfiguration.Verbose = ELoggingOptions.PrintInternalDiagnostics;
                        break;
                    case "i":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgInput, (title) => LexConfiguration.InputFileTitle = title);
                        break;
                    case "o":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgOutput, (title) => LexConfiguration.OutputFileTitle = title);
                        break;
                    case "l":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgLog, (title) => LexConfiguration.LogFileTitle = title);
                        break;
                    default:
                        Console.WriteLine($"Unknown argument '{charAtPos}' string '{arg}'");
                        break;
                }
            }
            return result;
        }

        private EParseArgResult SetFileTitle(string arg, string command, Func<string, string> setTitle)
        {
            string nextArg = GetNextArg();
            string fileTitleArg = GetFileTitle(arg, nextArg, command);
            setTitle(fileTitleArg);
            return EvaluateNextArg(nextArg);
        }

        private string GetFileTitle(string arg, string nextArg, string command)
        {
            string fileTitle = nextArg;
            if (nextArg.StartsWith(Constants.ArgPrefix))
            {
                int titleOffset = arg.StartsWith(command) ? command.Length : 1;
                fileTitle = arg.Substring(titleOffset);
            }
            return fileTitle;
        }
        private EParseArgResult EvaluateNextArg(string NextArg)
        {
            EParseArgResult result = EParseArgResult.ArgPlusOneOK;
            if (NextArg.StartsWith(Constants.ArgPrefix))
            {
                result = EParseArgResult.ArgOK;
            }
            return result;
        }
    }
}
