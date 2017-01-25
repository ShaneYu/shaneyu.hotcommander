using System;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Commands.LaunchUrl;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    internal sealed class ReleasesCommand : LaunchUrlCommand<ReleasesConfiguration>
    {
        #region Constructor

        public ReleasesCommand()
            : base(new ReleasesConfiguration())
        {
        }

        #endregion
    }

    internal sealed class ReleasesConfiguration : HotCommandConfigurationBase, ILaunchUrlConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("14C811FF-F46F-4F9F-94EB-E85894EF20DE"); }
            set { }
        }

        public string Name
        {
            get { return "Releases"; }
            set { }
        }

        public string Description
        {
            get { return "Opens the HotCommander github releases webpage."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public string Url => "https://github.com/ShaneYu/shaneyu.hotcommander/releases";

        public string Browser => "Google Chrome";

        public string Arguments => null;

        public bool RunElevated => false;

        #endregion
    }
}
