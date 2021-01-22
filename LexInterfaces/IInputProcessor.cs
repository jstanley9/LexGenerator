using LexBatch.Analyzer;

namespace LexBatch.LexInterfaces
{
    public interface IInputProcessor
    {
        NFA GenerateNFA();
    }
}
