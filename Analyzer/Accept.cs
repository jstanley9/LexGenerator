namespace LexBatch.Analyzer
{
    public class Accept
    {
        public string AcceptString { get; set; }
        public EItemAnchor Anchor { get; set; }

        public Accept(string acceptString, EItemAnchor anchor)
        {
            AcceptString = acceptString;
            Anchor = anchor;
        }
    }
}
