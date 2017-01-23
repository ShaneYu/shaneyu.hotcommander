using System.Diagnostics;

using ShaneYu.HotCommander.Commands.LaunchExecutable;

namespace ShaneYu.HotCommander.Commands.Cmd
{
    /// <summary>
    /// Cmd Command
    /// </summary>
    public class CmdCommand : CmdCommand<CmdConfiguration>
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The cmd configuration instance to use</param>
        public CmdCommand(CmdConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion
    }

    /// <summary>
    /// Cmd Command
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type to use</typeparam>
    public class CmdCommand<TConfiguration> : LaunchExecutableCommand<TConfiguration> 
        where TConfiguration : ICmdConfiguration
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration instance to use</param>
        public CmdCommand(TConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion

        #region Overrides

        protected override void BeforeExecution(ProcessStartInfo processStartInfo)
        {
            processStartInfo.Arguments = (Configuration.RemainOpen ? "/K " : "/C ") + Configuration.Command;

            if (!string.IsNullOrWhiteSpace(Configuration.WorkingDirectory))
                processStartInfo.WorkingDirectory = Configuration.WorkingDirectory;
        }

        #endregion
    }
}
