using System.Diagnostics;

namespace ShaneYu.HotCommander.Commands.LaunchExecutable
{
    /// <summary>
    /// Launch Executable Command
    /// </summary>
    public class LaunchExecutableCommand : LaunchExecutableCommand<LaunchExecutableConfiguration>
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The command configuration to use</param>
        public LaunchExecutableCommand(LaunchExecutableConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion
    }

    /// <summary>
    /// Launch Executable Command
    /// </summary>
    /// <typeparam name="TConfiguration">Determines the configuration type for the command to use</typeparam>
    public class LaunchExecutableCommand<TConfiguration> : HotCommandBase<TConfiguration>
        where TConfiguration : ILaunchExecutableConfiguration
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The command configuration to use</param>
        public LaunchExecutableCommand(TConfiguration configuration)
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
            var process = new Process();
            var processStartInfo = new ProcessStartInfo { FileName = Configuration.ExecutablePath };

            if (!string.IsNullOrWhiteSpace(Configuration.Arguments))
            {
                processStartInfo.Arguments = Configuration.Arguments;
            }

            if (Configuration.HideProcessWindow)
            {
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            if (Configuration.RunElevated)
            {
                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";
            }

            BeforeExecution(processStartInfo);

            process.StartInfo = processStartInfo;
            process.Start();

            AfterExecution(process);
        }

        #endregion

        #region Private Methods

        protected virtual void BeforeExecution(ProcessStartInfo processStartInfo)
        {
        }

        protected virtual void AfterExecution(Process process)
        {
        }

        #endregion
    }
}
