using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaneYu.HotCommander.Commands
{
    public class BasicCommandStep : HotCommandStepBase
    {
        public override bool IsRequired { get; }

        public override IEnumerable<string> Options { get; }

        public BasicCommandStep(string name, IHotCommandStep previousStep = null, bool isRequired = true, IEnumerable<string> options = null)
            : base(name, previousStep)
        {
            IsRequired = isRequired;
            Options = options;
        }

        public override void SetData(string data)
        {
            if (!string.IsNullOrWhiteSpace(data) && (Options == null || Options.Any(x => string.Equals(x, data, StringComparison.InvariantCultureIgnoreCase))))
            {
                base.SetData(data);
            }
        }
    }
}
