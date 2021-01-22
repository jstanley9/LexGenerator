using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    internal class LeXNFATable : INFATable
    {
        private IToken CurrentToken { get; set; }
        private ILineInput LineInput { get; set; }
        private Dictionary<string, string> Macros { get; set; }
        private List<NFA> NFAStates { get; set; }
        private IRuleInput RuleInput { get; set; }
        private NFA StartState { get; set; } = null;
        private static readonly IToken _EpsilonToken = new LeXToken(0, "", ETokenType.EPSILON);

        private static class StartEnd
        {
            public static NFA StartNFA { get; set; } = null;
            public static NFA EndNFA { get; set; } = null;
        }

        public LeXNFATable(ILineInput input, Dictionary<string, string> macros, List<NFA> nfa)
        {
            this.LineInput = input;
            this.Macros = macros;
            NFAStates = nfa;
        }

        private NFA AddToken(IToken token)
        {
            NFA nfa = new NFA(token);
            NFAStates.Add(nfa);
            return nfa;
        }

        /// <summary>
        /// The same translations that were needed in the expression rules are needed again here:
        /// <code>
        /// 
        ///     cat_expr -> cat_expr | factor
        ///                 factor
        ///                 
        /// </code>
        /// is translated to:
        /// <code>
        /// 
        ///     cat_expr  -> factor cat_expr'
        ///     cat_expr' -> | factor cat_expr'
        ///                  epsilon
        /// </code>
        /// </summary>
        /// <param name="startp"></param>
        /// <param name="endp"></param>
        private void Cat_Expr(ref NFA startp, ref NFA endp)
        {
            NFA e2_start = null;
            NFA e2_end = null;

            LexConfiguration.LogEnter($"Cat_Expr({NFA.Display(startp)}, {NFA.Display(endp)})");

            if (First_In_Cat(CurrentToken))
            {
                Factor(ref startp, ref endp);
            }

            while (First_In_Cat(CurrentToken))
            {
                Factor(ref e2_start, ref e2_end);
                endp.CopyFrom(e2_start);
                endp = e2_end;
            }

            LexConfiguration.LogExit($"Cat_Expr({NFA.Display(startp)}, {NFA.Display(endp)})");
        }

        private void DoDash(HashSet<char> bitSet)
        {
            char first = '\0';

            for (; !(Match(ETokenType.EOS)) && !(Match(ETokenType.CCL_END)); CurrentToken = RuleInput.Advance(CurrentToken))
            {
                if (!(Match(ETokenType.DASH)))
                {
                    first = CurrentToken.Token[0];
                    bitSet.Add(first);
                }
                else
                {
                    CurrentToken = RuleInput.Advance(CurrentToken);
                    for (; first <= CurrentToken.Token[0]; first++)
                    {
                        bitSet.Add(first);
                    }
                }
            }
        }

        /// <summary>
        /// Because a recursive descent compiler can't handle left recursion, the productions:
        /// 
        ///     expr -> expr OR cat_expr
        ///          |  cat_expr
        /// must be translated into:
        /// 
        ///     expr  -> cat_expr expr'
        ///     expr' -> OR cat_expr expr'
        ///              epsilon
        ///              
        ///  which can be implemented with this loop
        ///  
        ///     cat_expr
        ///     while (match(OR)
        ///         cat_expr
        ///         do the OR
        /// 
        /// </summary>
        private void Expr(ref NFA startp, ref NFA endp)
        {
            NFA e2_start = null;    // Expression to right of |
            NFA e2_end = null;
            NFA p;

            LexConfiguration.LogEnter($"Expr({NFA.Display(startp)}, {NFA.Display(endp)})");

            Cat_Expr(ref startp, ref endp);

            while (Match(ETokenType.OR))
            {
                CurrentToken = RuleInput.Advance(CurrentToken);
                Cat_Expr(ref startp, ref endp);

                p = AddToken(_EpsilonToken);
                p.Next2 = e2_start;
                p.Next = startp;
                startp = p;

                p = AddToken(_EpsilonToken);
                endp.Next = p;
                e2_end = p;
                endp = p;
            }
            LexConfiguration.LogExit($"Expr({NFA.Display(startp)}, {NFA.Display(endp)})");
        }

        private void Factor(ref NFA startp, ref NFA endp)
        {
            NFA start = null;
            NFA end = null;

            LexConfiguration.LogEnter($"Factor({NFA.Display(startp)}, {NFA.Display(endp)})");
            Term(ref startp, ref endp);

            if (Match(ETokenType.CLOSURE) || Match(ETokenType.PLUS_CLOSE) || Match(ETokenType.OPTIONAL))
            {
                start = AddToken(_EpsilonToken);
                end = AddToken(_EpsilonToken);
                start.Next = startp;
                endp.Next = end;

                if (Match(ETokenType.CLOSURE) || Match(ETokenType.OPTIONAL))
                {
                    start.Next2 = end;
                }

                if (Match(ETokenType.CLOSURE) || Match(ETokenType.PLUS_CLOSE))
                {
                    endp.Next2 = startp;
                }

                startp = start;
                endp = end;
                CurrentToken = RuleInput.Advance(CurrentToken);
            }
            LexConfiguration.LogExit($"Factor({NFA.Display(startp)}, {NFA.Display(endp)})");
        }

        private bool First_In_Cat(IToken token)
        {
            switch (token.TokenType)
            {
                case ETokenType.CLOSE_PAREN:
                case ETokenType.AT_EOL:
                case ETokenType.OR:
                case ETokenType.EOS:
                    return false;
                case ETokenType.CLOSURE:
                case ETokenType.PLUS_CLOSE:
                case ETokenType.OPTIONAL:
                    Parse_Err(EErrorCodes.Close);
                    return false;
                case ETokenType.CCL_END:
                    Parse_Err(EErrorCodes.Bracket);
                    return false;
                case ETokenType.AT_BOL:
                    Parse_Err(EErrorCodes.BOL);
                    return false;
            }
            return true;
        }

        /// <summary>
        /// The parser:
        ///     A simple recursive descent parser that creates a Thompson NFA for a regular expression.
        ///     The access routine (ParseRules) is below. The NFA is created as a direct graph, with each
        ///     node containing pointers to the next node.
        /// </summary>
        /// <param name="token">Initial token. Usually end of stream (EOS)</param>
        /// <returns>First state in the NFA graph. NFA = Nondeterministic Finite Automata </returns>
        private NFA Machine(IToken token)
        {
            LexConfiguration.LogEnter("Machine");

            NFA start = AddToken(_EpsilonToken);
            NFA p = start;

            NFA nextNFA = Rule();
            p.Next = nextNFA;

            while (!(Match(ETokenType.END_OF_INPUT)))
            {
                p.Next2 = AddToken(_EpsilonToken);
                p = p.Next2;
                p.Next = Rule();
            }

            LexConfiguration.LogExit();
            return start;
        }

        private bool Match(ETokenType tokenType) => CurrentToken.TokenType == tokenType;

        private void Parse_Err(EErrorCodes errorCode) => LexConfiguration.Error(CurrentToken.LineNumber, $"Token: {CurrentToken}", errorCode);

        public NFA ParseRules()
        {
            RuleInput = new LeXRuleInput(LineInput, Macros);
            CurrentToken = new LeXToken(0, String.Empty, ETokenType.EOS);
            /*          while (!(Match(ETokenType.END_OF_INPUT)))
                        {
                            CurrentToken = RuleInput.Advance(CurrentToken);
                        }*/
            CurrentToken = RuleInput.Advance(CurrentToken);
            StartState = Machine(CurrentToken);
            PrintNFA();
            return StartState;
        }

        private void PrintNFA()
        {
            LexConfiguration.LogFine("***** NFA Table *****");
            LexConfiguration.LogFine($"    Number of states = {NFAStates.Count}");
            LexConfiguration.LogFine($"    Start State = {StartState}");
            LexConfiguration.LogFine($"    List");
            int itemNbr = 0;
            foreach (NFA state in NFAStates)
            {
                LexConfiguration.LogFine($"{itemNbr++}: {state}");
            }
        }

        private NFA Rule()
        {
            LexConfiguration.LogEnter("Rule");

            NFA start = null;
            NFA end = null;
            EItemAnchor anchor = EItemAnchor.NoAnchor;

            if (Match(ETokenType.AT_BOL))
            {
                CurrentToken.Token = "\n";
                start = AddToken(_EpsilonToken);
                anchor = EItemAnchor.AnchorStart;
                CurrentToken = RuleInput.Advance(CurrentToken);
                NFA next = start.Next;
                Expr(ref next, ref end);
                start.Next = next;
            }
            else
            {
                Expr(ref start, ref end);
            }

            if (Match(ETokenType.AT_EOL))
            {
                // Pattern followed by a carriage-return or linefeed (use a character class)
                CurrentToken = RuleInput.Advance(CurrentToken);
                end.Next = AddToken(_EpsilonToken);
                end.Edge = EEdgeType.CCL;
                end.SetCharSet("\n");
                end = end.Next;
                end.Anchor = EItemAnchor.AnchorEnd;
            }

            end.Accept = RuleInput.CurrentAcceptActions;
            end.Anchor = anchor;
            CurrentToken = RuleInput.Advance(CurrentToken);

            LexConfiguration.LogExit();
            return start;
        }

        /// <summary>
        /// Process the term productions
        /// <code>
        /// 
        ///     term -> [...] | [^...] | [] | | [^] | . | (expr) | &lt;character&gt;
        ///     
        /// </code>
        /// The [] is nonstandard. It matches a space, tab, formfeed, or newline,
        /// but not a carriage return (\r). All of these are single nodes in the NFA.
        /// </summary>
        /// <param name="startp"></param>
        /// <param name="endp"></param>
        private void Term(ref NFA startp, ref NFA endp)
        {
            NFA start = null;

            LexConfiguration.LogEnter($"Term({NFA.Display(startp)}, {NFA.Display(endp)})");

            if (Match(ETokenType.OPEN_PAREN))
            {
                CurrentToken = RuleInput.Advance(CurrentToken);
                Expr(ref startp, ref endp);
                if (Match(ETokenType.CLOSE_PAREN))
                {
                    CurrentToken = RuleInput.Advance(CurrentToken);
                }
                else
                {
                    Parse_Err(EErrorCodes.Parenthesis);
                }
            }
            else
            {
                startp = start = AddToken(_EpsilonToken);
                endp = start.Next = AddToken(_EpsilonToken);

                if (!(Match(ETokenType.ANY) || Match(ETokenType.CCL_START)))
                {
                    start.Token = CurrentToken;
                    CurrentToken = RuleInput.Advance(CurrentToken);
                }
                else
                {
                    start.Edge = EEdgeType.CCL;
                    if (!(start.NewSet()))
                    {
                        Parse_Err(EErrorCodes.CCL);
                    }

                    if (Match(ETokenType.ANY)) // dot (.)
                    {
                        start.BitSetAddNewLine();
                        start.ComplementBitSet();
                    }
                    else
                    {
                        CurrentToken = RuleInput.Advance(CurrentToken);
                        if (Match(ETokenType.AT_BOL))
                        {
                            CurrentToken = RuleInput.Advance(CurrentToken);
                            start.BitSetAddNewLine();
                            start.ComplementBitSet();
                        }
                        if (!(Match(ETokenType.CCL_END)))
                        {
                            DoDash(start.BitSet);
                        }
                        else
                        {
                            for (char c = '\0'; c <= ' '; ++c)
                            {
                                start.BitSet.Add(c);
                            }
                        }
                    }
                    CurrentToken = RuleInput.Advance(CurrentToken);
                }
            }
            LexConfiguration.LogExit($"Term({NFA.Display(startp)}, {NFA.Display(endp)})");
        }
    }
}
