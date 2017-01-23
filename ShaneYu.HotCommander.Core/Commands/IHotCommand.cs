namespace ShaneYu.HotCommander.Commands
{
    /// <summary>
    /// Command Interface
    /// </summary>
    public interface IHotCommand : IHotCommand<IHotCommandConfiguration>
    {
    }

    /// <summary>
    /// Command Interface
    /// </summary>
    /// <typeparam name="TConfiguration">Determines the configuration type for the command</typeparam>
    public interface IHotCommand<out TConfiguration> where TConfiguration : IHotCommandConfiguration
    {
        /// <summary>
        /// Gets the command configuration.
        /// </summary>
        TConfiguration Configuration { get; }

        /// <summary>
        /// Gets the next step to the command.
        /// </summary>
        IHotCommandStep NextStep { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();
    }
}
