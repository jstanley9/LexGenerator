using System.ComponentModel;

namespace LexBatch.Analyzer
{
    public enum EErrorCodes
    {
        [Description("Not enough memory for NFA")]
        Memory,
        [Description("Malformed regular expression")]
        InvalidExpression,
        [Description("Missing close parenthesis")]
        Parenthesis,
        [Description("Internal error: Discard stack full")]
        StackFull,
        [Description("Too many regular expressions or expression too long")]
        Length,
        [Description("Missing [ in character class")]
        Bracket,
        [Description("^ must be at start of expression or after [")]
        BOL,
        [Description("*, ?, or * must follow expression or subexpression")]
        Close,
        [Description("Too many characters in accept actions")]
        Strings,
        [Description("Newline in quoted string, use \n to get newline into expression")]
        Newline,
        [Description("Missing } in macro expansion")]
        BadMacro,
        [Description("Macro does not exist")]
        NoMacro,
        [Description("Macro expansion nested too deeply")]
        MacroDepth,
        [Description("Quotes not closed before end of rule")]
        OpenQuote,
        [Description("Escape character last character in the rule")]
        OpenEscape,
        [Description("CCL set already allocated")]
        CCL
    }
}
