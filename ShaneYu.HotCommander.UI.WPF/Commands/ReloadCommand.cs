using System;
using System.ComponentModel;

using Autofac;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.IoC;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    [InternalHotCommand]
    internal sealed class ReloadCommand : HotCommandBase<ReloadConfiguration>
    {
        #region Constructors

        public ReloadCommand()
            : base(new ReloadConfiguration())
        {
        }

        #endregion

        #region Overrides

        public override async void Execute()
        {
            var commandManager = DependencyResolver.Current.Resolve<IHotCommandManager>();
            await commandManager.ReloadAsync();
        }

        #endregion
    }

    internal sealed class ReloadConfiguration : IHotCommandConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("5B8B0485-CF01-4326-B87A-6F92E507EF3B"); }
            set { }
        }

        public string Name
        {
            get { return "Reload Commands"; }
            set { }
        }

        public string Description
        {
            get { return "Reloads all commands from the file system."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
