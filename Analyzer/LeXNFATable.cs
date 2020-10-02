using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    class LeXNFATable : INFATable
    {
        private IToken CurrentToken { get; set; }
        private bool EndOfInput { get; set; } = false;
        private bool InQuote { get; set; } = false;
        private ILineInput LineInput { get; set; }
        private Stack<string> LineStack { get; set; }
        private Dictionary<string, string> Macros { get; set; }
        private List<NFA> NFAStates { get; set; }
        private IRuleInput RuleInput { get; set; }
        private int StartState { get; set; } = 0;

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
        /*private void Expr()
        {
            NFA e2_start = null;
            NFA e2_end = null;
            NFA p;

            LexConfiguration.LogInfo("Expr()");

            CatExpr();

            while (Match(ETokenType.OR))
            {
                CurrentToken = RuleInput.Advance(CurrentToken);
                CatExpr();

                p = new NFA(CurrentToken);
                p.Next2 = e2_start;
                p.Next = 
            }
        }*/

        /// <summary>
        /// The parser:
        ///     A simple recursive descent parser that creates a Thompson NFA for a regular expression.
        ///     The access routine (ParseRules) is below. The NFA is created as a direct graph, with each
        ///     node containing pointers to the next node.
        /// </summary>
        /// <param name="token">Initial token. Usually end of stream (EOS)</param>
        /// <returns>First state in the NFA graph. NFA = Nondeterministic Finite Automata </returns>
        /*private int Machine(IToken token)
        {
            LexConfiguration.LogEnter("Machine");
            NFA nfa = AddToken(token);
            int startState = NFAStates.Count - 1;
            NFA nextNFA = Rule(nfa);
            nfa.Next = nextNFA;

            while (!(EndOfInput))
            {
                nextNFA.Next2 = nextNFA;
            }

            LexConfiguration.LogExit();
            return startState;
        }*/

        private bool Match(ETokenType tokenType) => CurrentToken.TokenType == tokenType;

        public int ParseRules()
        {
            RuleInput = new LeXRuleInput(LineInput, Macros);
            CurrentToken = new LeXToken(0, String.Empty, ETokenType.EOS);
            while (!(Match(ETokenType.END_OF_INPUT)))
            {
                CurrentToken = RuleInput.Advance(CurrentToken);
            }
            //StartState = Machine(CurrentToken);
            return 0;
        }

        /*private NFA Rule(NFA currentRule)
        {
            NFA start = null;
            EItemAnchor anchor = EItemAnchor.NoAnchor;

            if (Match(ETokenType.AT_BOL))
            {
                CurrentToken.Token = "\n";
                start = AddToken(CurrentToken);
                anchor = EItemAnchor.AnchorStart;
                start.Anchor = anchor;
                CurrentToken = RuleInput.Advance(CurrentToken);
                Expr();
                start.Next = StartEnd.StartNFA;
            }
            else
            {
                Expr();
                start = StartEnd.StartNFA;
            }
            NFA end = StartEnd.EndNFA;

            if (Match(ETokenType.AT_EOL))
            {
                // Pattern followed by a carriage-return or linefeed (use a character class)
                CurrentToken = RuleInput.Advance(CurrentToken);
                end.Next = AddToken(CurrentToken);
                end.Edge = EEdgeType.CCL;
                end.SetCharSet("\n");
                end = end.Next;
                end.Anchor = EItemAnchor.AnchorEnd;
            }

            end.Accept = RuleInput.CurrentAcceptActions;
            end.Anchor = anchor;
            CurrentToken = RuleInput.Advance(CurrentToken);

            return start;
        }*/
    }
}
