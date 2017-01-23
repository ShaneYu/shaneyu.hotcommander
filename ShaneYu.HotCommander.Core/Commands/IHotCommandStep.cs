using System.Collections.Generic;

namespace ShaneYu.HotCommander.Commands
{
    public interface IHotCommandStep
    {
        string Name { get; }

        IHotCommandStep NextStep { get; }

        IHotCommandStep PreviousStep { get; }

        IEnumerable<string> Options { get; }

        string Data { get; }

        bool IsSet { get; }

        bool IsRequired { get; }

        void SetData(string data);

        void Reset();
    }
}
