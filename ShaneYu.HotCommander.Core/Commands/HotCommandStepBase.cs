using System.Collections.Generic;

namespace ShaneYu.HotCommander.Commands
{
    public class HotCommandStepBase : IHotCommandStep
    {
        public string Name { get; }

        public string Data { get; private set; }

        public bool IsSet { get; private set; }

        public virtual bool IsRequired => false;

        public virtual IEnumerable<string> Options => null;

        public virtual IHotCommandStep NextStep => null;

        public IHotCommandStep PreviousStep { get; }

        public HotCommandStepBase(string name, IHotCommandStep previousStep = null)
        {
            Name = name;
            PreviousStep = previousStep;
        }

        public virtual void SetData(string data)
        {
            Data = data;
            IsSet = true;
        }

        public virtual void Reset()
        {
            Data = null;
            IsSet = false;

            NextStep?.Reset();
        }
    }
}
