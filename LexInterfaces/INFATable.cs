using LexBatch.Analyzer;

namespace LexBatch.LexInterfaces
{
    internal interface INFATable
    {
        NFA ParseRules();
    }
}
