using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands.LaunchExecutable;
using ShaneYu.HotCommander.Validation;

namespace ShaneYu.HotCommander.Commands.Cmd
{
    /// <summary>
    /// Cmd Command Configuration Interface
    /// </summary>
    public interface ICmdConfiguration : ILaunchExecutableConfiguration
    {
        /// <summary>
        /// Gets the command line.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Gets whether the command process window should remain open after execution.
        /// </summary>
        bool RemainOpen { get; }
    }

    /// <summary>
    /// Cmd Command Configuration
    /// </summary>
    [Serializable]
    [DisplayGroupOrder("Core", 0)]
    [DisplayGroupOrder("Command Line", 1)]
    public class CmdConfiguration : HotCommandConfigurationBase, ICmdConfiguration
    {
        #region Fields

        private Guid _id;
        private string _name;
        private string _description;
        private bool _isEnabled = true;

        private bool _runElevated;
        private string _command;
        private string _workingDirectory;
        private bool _remainOpen;

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
        /// Gets or sets the command line.
        /// </summary>
        [Required]
        [DisplayOrder(0)]
        [DisplayGroup("Command Line")]
        [Description("The command line to execute.")]
        public string Command
        {
            get { return _command; }
            set
            {
                if (string.Compare(_command, value, StringComparison.Ordinal) != 0)
                {
                    _command = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        [DisplayOrder(1)]
        [DisplayGroup("Command Line")]
        [Description("The working directory to use.")]
        [FolderSelector(Description = "Please select the folder you wish to use as the working directory.")]
        [CustomValidator(typeof(ResolvedValidator<IDirectoryExistsValidator>))]
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set
            {
                if (string.Compare(_workingDirectory, value, StringComparison.Ordinal) != 0)
                {
                    _workingDirectory = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the command is enabled.
        /// </summary>
        [Required]
        [DisplayOrder(2)]
        [DisplayGroup("Command Line")]
        [Description("Whether or not the command process window should remain open after execution.")]
        public bool RemainOpen
        {
            get { return _remainOpen; }
            set
            {
                if (_remainOpen != value)
                {
                    _remainOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to run the executable elevated (as administrator).
        /// </summary>
        [Required]
        [DisplayGroup("Command Line")]
        [DisplayOrder(3)]
        [Description("Run command as administrator (elevated).")]
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

        /// <summary>
        /// Gets the path to the executable.
        /// </summary>
        [JsonIgnore]
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        public string ExecutablePath => "cmd.exe";

        /// <summary>
        /// Gets the start arguments.
        /// </summary>
        [JsonIgnore]
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        public string Arguments => null;

        /// <summary>
        /// Gets whether to hide the process window for the executed executable.
        /// </summary>
        [JsonIgnore]
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        public bool HideProcessWindow => false;

        #endregion
    }
}
