using LexBatch.LexInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IToken Token { get; set; } = null;
        public EEdgeType Edge { get; set; } = EEdgeType.Empty;
        public NFA Next { get; set; } = null;
        public NFA Next2 { get; set; } = null;
        public int StateNumber { get; } = NumberStates++;

        public NFA(IToken token)
        {
            Token = token;
        }

        public bool HasAccept() => Accept.Trim().Length > 0;

        public bool HasNext() => Next != null;

        public bool HasNext2() => Next2 != null;

        public string GetCharSet() => Token == null ? String.Empty : Token.Token;

        public bool IsAnchored() => Anchor != EItemAnchor.NoAnchor;

        public bool IsAnchoredAtStart() => Anchor == EItemAnchor.AnchorStart || Anchor == EItemAnchor.AnchorBoth;

        public bool IsAnchoredAtEnd() => Anchor == EItemAnchor.AnchorEnd || Anchor == EItemAnchor.AnchorBoth;

        public bool IsAnchoredAtBothEnds() => Anchor == EItemAnchor.AnchorBoth;

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
    }
}
