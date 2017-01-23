using Autofac;

using ShaneYu.HotCommander.IoC;

namespace ShaneYu.HotCommander.Commands.Alias
{
    /// <summary>
    /// Alias Command
    /// </summary>
    public class AliasCommand : AliasCommand<AliasConfiguration>
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The alias configuration instance to use</param>
        public AliasCommand(AliasConfiguration configuration)
            : base(configuration)
        {
        }

        #endregion
    }

    /// <summary>
    /// Alias Command
    /// </summary>
    /// <typeparam name="TConfiguration">The configuraiton type to use</typeparam>
    public class AliasCommand<TConfiguration> : HotCommandBase<TConfiguration>
        where TConfiguration : IAliasConfiguration
    {
        #region Fields

        private readonly IHotCommandManager _commandManager;
        private IHotCommand<IHotCommandConfiguration> _aliasCommand;

        #endregion

        #region Properties

        public override IHotCommandStep NextStep => _aliasCommand?.NextStep;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration instance to use</param>
        public AliasCommand(TConfiguration configuration)
            : base(configuration)
        {
            _commandManager = DependencyResolver.Current.Resolve<IHotCommandManager>();
            UpdateAliasCommand();

            Configuration.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Configuration.TargetCommandId))
                {
                    UpdateAliasCommand();
                }
            };
        }

        #endregion

        #region Private Methods

        private void UpdateAliasCommand()
        {
            _aliasCommand = _commandManager.Get<IHotCommand<IHotCommandConfiguration>>(Configuration.TargetCommandId);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            _aliasCommand?.Execute();
        }

        #endregion
    }
}
