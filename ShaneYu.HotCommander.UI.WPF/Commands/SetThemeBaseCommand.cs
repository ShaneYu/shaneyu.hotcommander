using System;
using System.Linq;

using Autofac;

using MahApps.Metro;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.IoC;
using ShaneYu.HotCommander.UI.WPF.Settings;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    [InternalHotCommand]
    internal sealed class SetThemeBaseCommand : HotCommandBase<SetThemeBaseConfiguration>
    {
        #region Properties

        public override IHotCommandStep NextStep { get; }

        #endregion

        #region Constructors

        public SetThemeBaseCommand()
            : base(new SetThemeBaseConfiguration())
        {
            NextStep = new BasicCommandStep("Theme", options: ThemeManager.AppThemes.Select(x => x.Name.Substring(4)));
        }

        #endregion

        #region Overrides

        public override async void Execute()
        {
            if (!NextStep.IsSet)
            {
                return;
            }

            var applicationSettingsProvider = DependencyResolver.Current.Resolve<ApplicationSettingsProvider>();

            applicationSettingsProvider.Settings.ThemeBase = NextStep.Data;
            await applicationSettingsProvider.SaveAsync();
        }

        #endregion
    }

    internal sealed class SetThemeBaseConfiguration : HotCommandConfigurationBase, IHotCommandConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("4B189CD0-5CA8-46FF-9857-2202DB8902B3"); }
            set { }
        }

        public string Name
        {
            get { return "Set Theme"; }
            set { }
        }

        public string Description
        {
            get { return "Allows the user to change the theme."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion
    }
}
