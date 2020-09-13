using LexBatch.LexInterfaces;
using LexGenerator.Analyzer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LexBatch.Analyzer
{
    public class ProcessArguments
    {
        private int ArgIndex { get; set; } = 0;
        private readonly string[] args;
        private readonly ILexConfigure config;

        public ProcessArguments(string[] args, ILexConfigure config)
        {
            this.args = args;
            this.config = config;
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
                        case ParseArgResult.ArgOK:
                            break;
                        case ParseArgResult.ArgPlusOneOK:
                            ArgIndex++;
                            break;
                        case ParseArgResult.ArgIncorrect:
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

        private ParseArgResult ParseArgument(string arg)
        {
            bool continueEvaluation = true;
            ParseArgResult result = ParseArgResult.ArgOK;
            for (int pos = 1; (continueEvaluation && pos < arg.Length); pos++)
            {
                string charAtPos = arg.Substring(pos, 1);
                switch (charAtPos)
                {
                    case "v":
                        config.Verbose = LoggingOptions.PrintStatistics;
                        break;
                    case "V":
                        config.Verbose = LoggingOptions.PrintInternalDiagnostics;
                        break;
                    case "i":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgInput,  (title) => config.InputFileTitle = title);
                        break;
                    case "o":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgOutput, (title) => config.OutputFileTitle = title);
                        break;
                    case "l":
                        continueEvaluation = false;
                        result = SetFileTitle(arg.Substring(pos), Constants.ArgLog, (title) => config.LogFileTitle = title);
                        break;
                    default:
                        Console.WriteLine($"Unknown argument '{charAtPos}' string '{arg}'");
                        break;
                }
            }
            return result;
        }

        private ParseArgResult SetFileTitle(string arg, string command, Func<string, string> setTitle)
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
        private ParseArgResult EvaluateNextArg(string NextArg)
        {
            ParseArgResult result = ParseArgResult.ArgPlusOneOK;
            if (NextArg.StartsWith(Constants.ArgPrefix))
            {
                result = ParseArgResult.ArgOK;
            }
            return result;
        }
    }
}
