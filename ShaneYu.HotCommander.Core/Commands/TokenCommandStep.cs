using System.Collections.Generic;

using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander.Core.Commands
{
    public class TokenCommandStep : HotCommandStepBase
    {
        private TokenCommandStep _nextTokenStep;

        public override IHotCommandStep NextStep => _nextTokenStep;

        public override bool IsRequired { get; }

        public override IEnumerable<string> Options { get; }

        public TokenCommandStep(TokenBit tokenBit, IHotCommandStep previousStep = null)
            : base(tokenBit.Name, previousStep)
        {
            IsRequired = tokenBit.Default == null;
            Options = tokenBit.Options;
        }

        public void SetNextTokenStep(TokenCommandStep nextTokenStep)
        {
            _nextTokenStep = nextTokenStep;
        }
    }

    public struct TokenBit
    {
        public string Name { get; set; }

        public string Default { get; set; }

        public IEnumerable<string> Options { get; set; }
    }
}
