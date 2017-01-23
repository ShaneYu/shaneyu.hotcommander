namespace ShaneYu.HotCommander.Commands
{
    /// <summary>
    /// Command Base
    /// </summary>
    /// <typeparam name="TConfiguration">Determines the configuration type for the command</typeparam>
    public abstract class HotCommandBase<TConfiguration> : IHotCommand<TConfiguration> where TConfiguration : IHotCommandConfiguration
    {
        /// <summary>
        /// Gets the command configuration.
        /// </summary>
        public TConfiguration Configuration { get; }


        /// <summary>
        /// Gets the next step to the command.
        /// </summary>
        public virtual IHotCommandStep NextStep => null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The command configuration instance</param>
        protected HotCommandBase(TConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void Execute();
    }
}
