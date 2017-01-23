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
    internal sealed class SetThemeAccentCommand : HotCommandBase<SetThemeAccentConfiguration>
    {
        #region Properties

        public override IHotCommandStep NextStep { get; }

        #endregion

        #region Constructors

        public SetThemeAccentCommand()
            : base(new SetThemeAccentConfiguration())
        {
            NextStep = new BasicCommandStep("Color", options: ThemeManager.Accents.Select(x => x.Name));
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

            applicationSettingsProvider.Settings.AccentColor = NextStep.Data;
            await applicationSettingsProvider.SaveAsync();
        }

        #endregion
    }

    internal sealed class SetThemeAccentConfiguration : HotCommandConfigurationBase, IHotCommandConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("32C2D398-75EA-41F9-8564-8050CA1FACE4"); }
            set { }
        }

        public string Name
        {
            get { return "Set Accent"; }
            set { }
        }

        public string Description
        {
            get { return "Allows the user to change the accent color."; }
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
