using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ShaneYu.HotCommander.Core.Commands;
using ShaneYu.HotCommander.Helpers;

namespace ShaneYu.HotCommander.Commands.LaunchUrl
{
    /// <summary>
    /// Launch URL Command
    /// </summary>
    public class LaunchUrlCommand : LaunchUrlCommand<LaunchUrlConfiguration>
    {
        #region Fields

        private IHotCommandStep _nextStep;

        #endregion

        #region Properties

        public override IHotCommandStep NextStep => _nextStep;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The command configuration to use</param>
        public LaunchUrlCommand(LaunchUrlConfiguration configuration)
            : base(configuration)
        {
            ParseTokenBits();
        }

        #endregion

        #region Private Methods

        private void ParseTokenBits()
        {
			if (string.IsNullOrEmpty(Configuration.Url))
			{
				return;
			}

            var tokenBits = new List<TokenBit>();

            foreach (var match in Regex.Matches(Configuration.Url, @"\{(?<token>.*?)\}").Cast<Match>())
            {
                var token = match.Groups["token"].Value;
                var tokenParts = token.Split(':');

                var name = tokenParts[0];
                IEnumerable<string> opts = null;
                string defaultValue = null;

                if (tokenParts.Length > 1)
                {
                    for (var i = 1; i < tokenParts.Length; i++)
                    {
                        if (tokenParts[i].StartsWith("["))
                        {
                            opts = tokenParts[i].Substring(1, tokenParts[i].Length - 2).Split(',');
                        }
                        else
                        {
                            defaultValue = tokenParts[i];
                        }
                    }
                }

                tokenBits.Add(new TokenBit
                {
                    Name = name,
                    Options = opts,
                    Default = defaultValue
                });
            }

            if (tokenBits.Any())
            {
                _nextStep = new TokenCommandStep(tokenBits[0]);
                var tokenStep = (TokenCommandStep)_nextStep;

                for (var i = 1; i < tokenBits.Count; i++)
                {
                    tokenStep.SetNextTokenStep(new TokenCommandStep(tokenBits[i], tokenStep));
                    tokenStep = (TokenCommandStep)tokenStep.NextStep;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Launch URL Command
    /// </summary>
    /// <typeparam name="TConfiguration">Determines the configuration type for the command to use</typeparam>
    public class LaunchUrlCommand<TConfiguration> : HotCommandBase<TConfiguration>
        where TConfiguration : ILaunchUrlConfiguration
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The command configuration to use</param>
        public LaunchUrlCommand(TConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            var browser = BrowserHelper.GetBrowserOrDefault(Configuration.Browser);

            if (browser == null)
            {
                throw new InvalidOperationException("There are no browsers installed on this machine.");
            }

            var process = new Process
            {
                StartInfo =
                {
                    FileName = browser.ExecutablePath,
                    Arguments = $"{Configuration.Url} {Configuration.Arguments ?? ""}"
                }
            };

            if (Configuration.RunElevated)
                process.StartInfo.Verb = "runas";

            process.Start();
        }

        #endregion
    }
}
