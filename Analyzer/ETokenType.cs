using System.ComponentModel;

namespace LexBatch.Analyzer
{
    public enum ETokenType
    {
        [Description("")]
        EOS,
        [Description(".")]
        ANY,
        [Description("^")]
        AT_BOL,
        [Description("$")]
        AT_EOL,
        [Description("]")]
        CCL_END,
        [Description("[")]
        CCL_START,
        [Description("}")]
        CLOSE_CURLY,
        [Description(")")]
        CLOSE_PAREN,
        [Description("*")]
        CLOSURE,
        [Description("-")]
        DASH,
        [Description("")]
        END_OF_INPUT,
        [Description("")]
        L,
        [Description("{")]
        OPEN_CURLY,
        [Description("(")]
        OPEN_PAREN,
        [Description("?")]
        OPTIONAL,
        [Description("|")]
        OR,
        [Description("+")]
        PLUS_CLOSE
    }
}
