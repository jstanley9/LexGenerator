using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;

namespace LexBatch.Analyzer
{
    public class NFA
    {
        public static int NumberStates { get; private set; } = 0;

        public string Accept { get; set; } = "";
        private EItemAnchor anchor = EItemAnchor.NoAnchor;
        public EItemAnchor Anchor
        {
            get { return anchor; }
            set { SetAnchor(value); }
        }

        private IToken _token = null;
        public IToken Token
        {
            get { return _token; }
            set { SetToken(value); }
        }
        public EEdgeType Edge { get; set; } = EEdgeType.Empty;
        public NFA Next { get; set; } = null;
        public NFA Next2 { get; set; } = null;
        public HashSet<char> BitSet { get; private set; } = null;
        public int StateNumber { get; private set; } = NumberStates++;

        public NFA() => Token = null;

        public NFA(IToken token)
        {
            if (token != null)
            {
                Token = token;
                Edge = (token.IsTokenType(ETokenType.EPSILON)) ? Edge = EEdgeType.Epsilon : Edge = EEdgeType.Token;
            }
            else
            {
                Token = null;
            }
        }

        public static string Display(NFA toDisplay)
        {
            if (toDisplay == null)
            {
                return "null";
            }
            else
            {
                return toDisplay.ToString();
            }
        }

        public void BitSetAddNewLine()
        {
            string newList = Environment.NewLine;
            foreach (char value in newList)
            {
                BitSet.Add(value);
            }
        }

        public void CopyFrom(NFA source)
        {
            Accept = source.Accept;
            Anchor = source.Anchor;
            Token = source.Token;
            Edge = source.Edge;
            Next = source.Next;
            Next2 = source.Next2;
            BitSet = source.BitSet;
            StateNumber = source.StateNumber;
        }

        public void ComplementBitSet() => Edge = Edge == EEdgeType.CCL ? EEdgeType.CCLComplement : EEdgeType.CCL;

        public bool HasAccept() => Accept.Trim().Length > 0;

        public bool HasNext() => Next != null;

        public bool HasNext2() => Next2 != null;

        public bool HasTransition(char Transition_Char) => (Token.HasCharacter(Transition_Char) ||
                                                            ((BitSet != null) && BitSet.Contains(Transition_Char)));

        public string GetCharSet() => Token == null ? String.Empty : Token.Token;

        public bool IsAnchored() => Anchor != EItemAnchor.NoAnchor;

        public bool IsAnchoredAtStart() => Anchor == EItemAnchor.AnchorStart || Anchor == EItemAnchor.AnchorBoth;

        public bool IsAnchoredAtEnd() => Anchor == EItemAnchor.AnchorEnd || Anchor == EItemAnchor.AnchorBoth;

        public bool IsAnchoredAtBothEnds() => Anchor == EItemAnchor.AnchorBoth;

        public bool NewSet()
        {
            if (BitSet == null)
            {
                BitSet = new HashSet<char>();
                Edge = EEdgeType.CCL;
                return true;
            }
            else
            {
                return false;
            }
        }

        private string NextReference(NFA next) => next == null ? "null" : $"{next.StateNumber}";

        private void SetAnchor(EItemAnchor newAnchor)
        {
            switch (Anchor)
            {
                case EItemAnchor.NoAnchor:
                    {
                        anchor = newAnchor;
                        break;
                    }
                case EItemAnchor.AnchorStart:
                    {
                        if (newAnchor == EItemAnchor.AnchorEnd)
                        {
                            anchor = EItemAnchor.AnchorBoth;
                        }
                        else
                        {
                            anchor = newAnchor;
                        }
                        break;
                    }
                case EItemAnchor.AnchorEnd:
                    {
                        if (newAnchor == EItemAnchor.AnchorStart)
                        {
                            anchor = EItemAnchor.AnchorBoth;
                        }
                        else
                        {
                            anchor = newAnchor;
                        }
                        break;
                    }
                case EItemAnchor.AnchorBoth:
                    {
                        if (newAnchor == EItemAnchor.NoAnchor)
                        {
                            anchor = newAnchor;
                        }
                        break;
                    }
                default:
                    {
                        anchor = newAnchor;
                        break;
                    }
            }
        }

        public void SetCharSet(string charSet)
        {
            if (Token != null && Token.Token != charSet)
            {
                Token = new LeXToken(Token.LineNumber, charSet, Token.TokenType);
            }
        }

        private void SetToken(IToken value)
        {
            if (value != null)
            {
                _token = value;
                Edge = EEdgeType.Token;
            }
            else
            {
                _token = null;
                Edge = EEdgeType.Epsilon;
            }
        }

        public override string ToString()
        {
            string charSet = "";
            if (BitSet != null && BitSet.Count > 0)
            {
                foreach (char value in BitSet)
                {
                    charSet += value;
                }
                charSet = charSet.Replace("\n", "\\n").Replace("\r", "\\r");
            }
            return $"{{Accept:{Accept.Replace("\n", "\\n").Replace("\r", "\\r")} Anchor:{LexConfiguration.GetEnumName(Anchor)} Token:{{{_token}}} " +
                       $"Edge:{LexConfiguration.GetEnumName(Edge)} Next:{NextReference(Next)} Next2:{NextReference(Next2)} " +
                       $"BitSet:{charSet} StateNumber:{StateNumber}}}";
        }
    }
}
