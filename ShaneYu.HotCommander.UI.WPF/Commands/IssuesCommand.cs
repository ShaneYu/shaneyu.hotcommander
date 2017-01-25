using System;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Commands.LaunchUrl;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    internal sealed class IssuesCommand : LaunchUrlCommand<IssuesConfiguration>
    {
        #region Constructor

        public IssuesCommand()
            : base(new IssuesConfiguration())
        {
        }

        #endregion
    }

    internal sealed class IssuesConfiguration : HotCommandConfigurationBase, ILaunchUrlConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("BF6AB307-D426-4C8A-A375-AD4FA7B83AED"); }
            set { }
        }

        public string Name
        {
            get { return "Issues"; }
            set { }
        }

        public string Description
        {
            get { return "Opens the HotCommander github issues webpage."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public string Url => "https://github.com/ShaneYu/shaneyu.hotcommander/issues";

        public string Browser => "Google Chrome";

        public string Arguments => null;

        public bool RunElevated => false;

        #endregion
    }
}
