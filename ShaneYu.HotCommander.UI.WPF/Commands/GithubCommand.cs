using System;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Commands.LaunchUrl;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    internal sealed class GithubCommand : LaunchUrlCommand<GithubConfiguration>
    {
        #region Constructor

        public GithubCommand()
            : base(new GithubConfiguration())
        {
        }

        #endregion
    }

    internal sealed class GithubConfiguration : HotCommandConfigurationBase, ILaunchUrlConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("43475E93-AAEE-4683-B4D1-A251C4C5578D"); }
            set { }
        }

        public string Name
        {
            get { return "Github"; }
            set { }
        }

        public string Description
        {
            get { return "Opens the HotCommander github webpage."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public string Url => "https://github.com/ShaneYu/shaneyu.hotcommander";

        public string Browser => "Google Chrome";

        public string Arguments => null;

        public bool RunElevated => false;

        #endregion
    }
}
