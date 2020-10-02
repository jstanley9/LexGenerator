using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class LeXRuleInput : IRuleInput
    {
        public string CurrentAcceptActions { get; private set; }
        private int CurrentPosition { get; set; }
        private string CurrentRule { get; set; }
        private bool IsInQuote { get; set; } = false;
        private ILineInput LineInput { get; }
        private Dictionary<string, string> Macros { get; }
        private Stack<StackedScanner> RuleStack { get; } = new Stack<StackedScanner>();
        private readonly Dictionary<string, ETokenType> TokenDict = new Dictionary<string, ETokenType>();

        private class StackedScanner
        {
            public int Position { get; }
            public string Rule { get; }

            public StackedScanner(int position, string rule)
            {
                this.Position = position;
                this.Rule = rule;
            }
        }

        public LeXRuleInput(ILineInput lineInput, Dictionary<string, string> macros)
        {
            this.LineInput = lineInput;
            this.Macros = macros;

            SetMap();
        }

        public IToken Advance(IToken lastToken)
        {
            IToken returnToken = new LeXToken(LineInput.LineNumber, String.Empty, ETokenType.EOS);

            // Load a new rule as needed. Handles blank lines
            if (lastToken.IsTokenType(ETokenType.EOS))
            {
                if (IsInQuote)
                {
                    Error(LineInput.LineNumber, CurrentRule, EErrorCodes.OpenQuote);
                    IsInQuote = false;
                }
                GetNextRule(returnToken);
                if (returnToken.IsTokenType(ETokenType.END_OF_INPUT))
                {
                    LexConfiguration.LogInfo($"Token: {returnToken}");
                    return returnToken;
                }
            }

            // Pop macro expansion stack when end of macro reached
            while (IsEndOfRule() && RuleStack.Count > 0)
            {
                PopRuleStack();
            }

            // Check for macro expansion as long as we are not in a quote
            if (!(IsInQuote))
            {
                while ((CurrentPosition + 1) < CurrentRule.Length && GetToken(false).Equals(Constants.OPEN_CURLY_BRACE))
                {
                    ExpandMacro();
                }
            }

            // Handle quotes
            string thisChar = GetToken(true);
            if (thisChar.Equals(Constants.QUOTE))
            {
                IsInQuote = !(IsInQuote);
                thisChar = GetToken(true);
                if (IsEndOfRule())
                {
                    returnToken.TokenType = ETokenType.EOS;
                    LexConfiguration.LogInfo($"Token: {returnToken}");
                    return returnToken;
                }
            }

            //
            bool sawEscape = thisChar.Equals(Constants.ESCAPE);
            if (sawEscape)
            {
                if (IsEndOfRule())
                {
                    Error(LineInput.LineNumber, CurrentRule, EErrorCodes.OpenEscape);
                    IsInQuote = false;
                }
                thisChar = GetToken(true);
            }

            if (thisChar.Length == 0 && IsEndOfRule())
            {
                returnToken.TokenType = ETokenType.EOS;
            }
            else
            {
                returnToken.Token = thisChar;
                returnToken.TokenType = MapToken2ToTokenType(thisChar, sawEscape);
            }
            LexConfiguration.LogInfo($"Token: {returnToken}");
            return returnToken;
        }

        private void Error(int lineNumber, string text, EErrorCodes error)
        {
            LexConfiguration.Error(lineNumber, text, error);
        }

        /// <summary>
        ///  Pushes the current scanner onto the stack. It then replaces the current scanner with the body of the macro.
        ///  A macro can have an empty body. If the requested macro has not been defined and error is issued and an empty
        ///  body is returned.
        /// </summary>
        private void ExpandMacro()
        {
            int closePosition = CurrentRule.IndexOf(Constants.CLOSE_CURLY_BRACE, CurrentPosition);
            if (closePosition < 0)
            {
                Error(LineInput.LineNumber, CurrentRule.Substring(CurrentPosition), EErrorCodes.BadMacro);
            }
            else
            {
                CurrentPosition++;
                string macroName = CurrentRule.Substring(CurrentPosition, closePosition - CurrentPosition);
                CurrentPosition = closePosition + 1;
                RuleStack.Push(new StackedScanner(CurrentPosition, CurrentRule));
                CurrentPosition = 0;
                if (Macros.ContainsKey(macroName))
                {
                    CurrentRule = Macros[macroName];
                }
                else
                {
                    CurrentRule = String.Empty;
                    Error(LineInput.LineNumber, CurrentRule.Substring(CurrentPosition), EErrorCodes.NoMacro);
                }
            }
        }

        private int GetEndOfRule(string rule)
        {
            for (int end = 1; end < rule.Length; end++)
            {
                if (Char.IsWhiteSpace(rule[end]))
                {
                    return end;
                }
            }
            return rule.Length;
        }

        private void GetNextRule(IToken returnToken)
        {
            CurrentRule = String.Empty;
            CurrentPosition = 0;
            while (!(LineInput.NextLineStartsWith(Constants.RulesBoundary)))
            {
                ISourceLine rule = LineInput.ReadLine();
                CurrentRule = rule.Line;
                if (Char.IsWhiteSpace(CurrentRule[CurrentPosition]))
                {
                    if (CurrentRule.Trim().Length > 0)
                    {
                        Error(rule.LineNumber, CurrentRule, EErrorCodes.InvalidExpression);
                    }
                }
                else
                {
                    returnToken.LineNumber = rule.LineNumber;
                    CurrentAcceptActions = String.Empty;
                    int endOfRule = GetEndOfRule(CurrentRule);
                    if (endOfRule < CurrentRule.Length)
                    {
                        CurrentAcceptActions = TrimWhiteSpace(CurrentRule.Substring(endOfRule)) + Environment.NewLine;
                        CurrentRule = CurrentRule.Substring(0, endOfRule);
                    }
                }
                while (LineInput.NextLineStartsWith(Constants.SPACE))
                {
                    ISourceLine nextAccept = LineInput.ReadLine();
                    CurrentAcceptActions += nextAccept.Line.Trim() + Environment.NewLine;
                }
                return;
            }
            returnToken.TokenType = ETokenType.END_OF_INPUT;
        }

        private int GetStartOfAccept(string rule, int endOfRule)
        {
            for (int start = endOfRule; start < rule.Length; start++)
            {
                if (!(Char.IsWhiteSpace(rule[start])))
                {
                    return start;
                }
            }
            return rule.Length;
        }

        private string GetToken(Boolean increment)
        {
            if (IsEndOfRule())
            {
                return String.Empty;
            }
            string token = CurrentRule.Substring(CurrentPosition, 1);
            if (increment)
            {
                CurrentPosition++;
            }
            return token;
        }

        private bool IsEndOfRule()
        {
            return (CurrentPosition >= CurrentRule.Length);
        }

        private ETokenType MapToken2ToTokenType(string currentChar, bool sawEscape)
        {
            if (sawEscape ^ IsInQuote || !(TokenDict.ContainsKey(currentChar)))
            {
                return ETokenType.L;
            }
            {
                return TokenDict[currentChar];
            }
        }

        private void PopRuleStack()
        {
            StackedScanner rule = RuleStack.Pop();
            CurrentPosition = rule.Position;
            CurrentRule = rule.Rule;
        }

        private void SetMap()
        {
            foreach (ETokenType value in (ETokenType[])Enum.GetValues(typeof(ETokenType)))
            {
                string description = LexConfiguration.GetEnumDescription(value);
                if (description.Length > 0)
                {
                    TokenDict.Add(description, value);
                }
            }
        }

        private string TrimWhiteSpace(string acceptAction)
        {
            int startOfAction = GetStartOfAccept(acceptAction, 0);
            for (int end = acceptAction.Length - 1; ; end--)
            {
                if (!(Char.IsWhiteSpace(acceptAction[end])))
                {
                    return acceptAction.Substring(startOfAction, end - startOfAction + 1);
                }
            }
        }

    }
}
