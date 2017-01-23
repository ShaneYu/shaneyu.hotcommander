using System;
using System.ComponentModel;

using Autofac;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.IoC;
using ShaneYu.HotCommander.UI.WPF.Extensions;
using ShaneYu.HotCommander.UI.WPF.Windows;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    [InternalHotCommand]
    internal sealed class ConfigureCommand : HotCommandBase<ConfigureConfiguration>
    {
        #region Fields

        private CommandCenter _commandCenter;

        #endregion

        #region Constructors

        public ConfigureCommand()
            : base(new ConfigureConfiguration())
        {
        }

        #endregion

        #region Overrides

        public override void Execute()
        {
            if (_commandCenter == null)
            {
                _commandCenter = DependencyResolver.Current.Resolve<CommandCenter>();
                _commandCenter.Closed += (sender, args) => _commandCenter = null;
                _commandCenter.ShowOnActiveMonitor();
                _commandCenter.Focus();
            }
            else
            {
                _commandCenter.ActivateOnActiveMonitor();
                _commandCenter.Focus();
            }
        }

        #endregion
    }

    internal sealed class ConfigureConfiguration : IHotCommandConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("806BDC23-F28E-4371-A8FE-1AE570961F25"); }
            set { }
        }

        public string Name
        {
            get { return "Configure"; }
            set { }
        }

        public string Description
        {
            get { return "Opens the command center for configuring custom commands and general settings."; }
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
