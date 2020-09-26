using LexBatch.LexInterfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexBatch.Analyzer
{
    public class LexInput : IInputProcessor
    {
        private readonly List<ISourceLine> codeConstants;
        private readonly List<NFA> NFAList;

        public LexInput(List<ISourceLine> codeConstants, List<NFA> NFAList)
        {
            this.codeConstants = codeConstants;
            this.NFAList = NFAList;
        }

        public int GenerateNFA()
        {
            ILineInput lineInput = new LeXLineInput();
            int initialState = -1;
            if (lineInput.FileIsOpen)
            {
                codeConstants.Clear();
                ReadHeadings(lineInput);

                Dictionary<string, string> macros = new Dictionary<string, string>(32);
                ReadMacros(lineInput, macros);
                initialState = GenerateNFA(lineInput, macros);

                ReadCodeLines(lineInput);
                lineInput.Close();
            }
            return initialState;
        }

        private int GenerateNFA(ILineInput lineInput, Dictionary<string, string> macros)
        {
            if (lineInput.NextLineStartsWith(Constants.RulesBoundary))
            {
                lineInput.ReadLine();
            }

            INFATable nfaStates = new LeXNFATable(lineInput, macros, NFAList);
            int startState = nfaStates.ParseRules();
            lineInput.ReadLine();
            return startState;
        }

        private void ReadCodeLines(ILineInput lineInput)
        {
            while (lineInput.HasNext())
            {
                codeConstants.Add(lineInput.ReadLine());
            }
        }

        private void ReadHeadings(ILineInput lineInput)
        {
            if (lineInput.NextLineStartsWith(Constants.PrefixStart))
            {
                while (!(lineInput.NextLineStartsWith(Constants.PrefixEnd)))
                {
                    codeConstants.Add(lineInput.ReadLine());
                }
                if (lineInput.NextLineStartsWith(Constants.PrefixEnd))
                {
                    codeConstants.Add(lineInput.ReadLine()); // This boundary will tell us when to create the class
                }
            }
        }

        /// A macro must start with the macro name at the first position in a line. The macro definition can continue on 
        /// one or more following lines.The first character of the continuation lines must be a space.End of line and space
        /// characters are removed when concatenating the continuation definition to the definition.
        /// Duplicate names result in the last macro with the name being the one used.
        private void ReadMacros(ILineInput lineInput, Dictionary<string, string> macros)
        {
            macros.Clear();
            while (!(lineInput.NextLineStartsWith(Constants.RulesBoundary)))
            {
                ISourceLine thisLine = lineInput.ReadLine();
                string line = thisLine.Line;
                while (lineInput.NextLineStartsWith(" "))
                {
                    ISourceLine nextLine = lineInput.ReadLine();
                    line = line.Trim() + nextLine.Line.Trim();
                }
                int macroNameEnd = line.IndexOf(" ");
                macros.Add(line.Substring(0, macroNameEnd), line.Substring(macroNameEnd).Trim());
            }
            LexConfiguration.LogFine($"Macros: {macros}");
        }
    }
}
