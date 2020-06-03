using avoCADo.Actions;

namespace avoCADo
{
    public interface IInstructionBuffer
    {
        IInstructionEntry LastInstruction { get; }
        void IssueInstruction<TInstruction, TParameters>(TParameters parameters) where TInstruction : Instruction<TParameters>, new();
        void Undo();

        void Clear();
    }
}