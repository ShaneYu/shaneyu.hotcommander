using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ShaneYu.HotCommander.Attributes;

namespace ShaneYu.HotCommander.Commands.LaunchUrl
{
    /// <summary>
    /// Launch Url Command Configuration Interface
    /// </summary>
    public interface ILaunchUrlConfiguration : IHotCommandConfiguration
    {
        /// <summary>
        /// Gets the URL to launch.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Gets the browser to use.
        /// </summary>
        string Browser { get; }

        /// <summary>
        /// Gets the arguments to launch with.
        /// </summary>
        string Arguments { get; }

        /// <summary>
        /// Gets whether to launch the browser as administrator (elevated).
        /// </summary>
        bool RunElevated { get; }
    }

    /// <summary>
    /// Launch Url Command Configuration
    /// </summary>
    [Serializable]
    [DisplayGroupOrder("Core", 0)]
    [DisplayGroupOrder("URL", 1)]
    public class LaunchUrlConfiguration : HotCommandConfigurationBase, ILaunchUrlConfiguration
    {
        #region Fields

        private Guid _id;
        private string _name;
        private string _description;
        private bool _isEnabled = true;

        private string _url;
        private string _arguments;
        private string _browser;
        private bool _runElevated;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the command ID.
        /// </summary>
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Readonly)]
        [DisplayOrder(0)]
        [DisplayGroup("Core")]
        [Description("The command unique ID.")]
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (!Equals(_id, value))
                {
                    _id = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        [Required]
        [StringLength(255)]
        [DisplayOrder(1)]
        [DisplayGroup("Core")]
        [Description("The command name.")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.Compare(_name, value, StringComparison.Ordinal) != 0)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the command description.
        /// </summary>
        [Required]
        [StringLength(255)]
        [DisplayOrder(2)]
        [DisplayGroup("Core")]
        [Description("The command description.")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (string.Compare(_description, value, StringComparison.Ordinal) != 0)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the command is enabled.
        /// </summary>
        [DisplayOrder(3)]
        [DisplayGroup("Core")]
        [Description("Whether or not this command is enabled.")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start arguments.
        /// </summary>
        [Required]
        [DisplayGroup("URL")]
        [DisplayOrder(0)]
        [Description("The URL to launch.")]
        [RegularExpression("\\w+:\\/\\/.*")]
        public string Url
        {
            get { return _url; }
            set
            {
                if (string.Compare(_url, value, StringComparison.Ordinal) != 0)
                {
                    _url = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the arguments to launch with.
        /// </summary>
        [DisplayGroup("URL")]
        [DisplayOrder(1)]
        [Description("The arguments to launch with.")]
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                if (_arguments != value)
                {
                    _arguments = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the browser to launch with.
        /// </summary>
        [DisplayGroup("URL")]
        [DisplayOrder(2)]
        [Description("The browser to launch with.")]
        [UIHint("BrowserSelector")]
        public string Browser
        {
            get { return _browser; }
            set
            {
                if (_browser != value)
                {
                    _browser = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to run the browser elevated (as administrator).
        /// </summary>
        [DisplayGroup("URL")]
        [DisplayOrder(3)]
        [Description("Run browser as administrator (elevated).")]
        public bool RunElevated
        {
            get { return _runElevated; }
            set
            {
                if (_runElevated != value)
                {
                    _runElevated = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}
