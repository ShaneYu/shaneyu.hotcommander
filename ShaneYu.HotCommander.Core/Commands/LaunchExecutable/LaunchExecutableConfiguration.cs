using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Validation;

namespace ShaneYu.HotCommander.Commands.LaunchExecutable
{
    /// <summary>
    /// Launch Executable Command Configuration Interface
    /// </summary>
    public interface ILaunchExecutableConfiguration : IHotCommandConfiguration
    {
        /// <summary>
        /// Gets the path to the executable.
        /// </summary>
        string ExecutablePath { get; }

        /// <summary>
        /// Gets the start arguments.
        /// </summary>
        string Arguments { get; }

        /// <summary>
        /// Gets whether to hide the process window for the executed executable.
        /// </summary>
        bool HideProcessWindow { get; }

        /// <summary>
        /// Gets whether to run the executable elevated (as administrator).
        /// </summary>
        bool RunElevated { get; }
    }

    /// <summary>
    /// Launch Executable Command Configuration
    /// </summary>
    [Serializable]
    [DisplayGroupOrder("Core", 0)]
    [DisplayGroupOrder("Executable", 1)]
    public class LaunchExecutableConfiguration : HotCommandConfigurationBase, ILaunchExecutableConfiguration
    {
        #region Fields

        private Guid _id;
        private string _name;
        private string _description;
        private bool _isEnabled = true;

        private string _executablePath;
        private string _arguments;
        private bool _runElevated;
        private bool _hideProcessWindow;

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
            get {  return _id; }
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
        /// Gets or sets the path to the executable.
        /// </summary>
        [Required]
        [StringLength(1024)]
        [RegularExpression(".*\\.(exe|bat)", ErrorMessage = "The selected executable must be a .exe or .bat file.")]
        [CustomValidator(typeof(ResolvedValidator<IFileExistsValidator>))]
        [DisplayGroup("Executable")]
        [DisplayOrder(0)]
        [DisplayName("Executable")]
        [Description("The executable to run when this command is executed.")]
        [FileSelector(
            Title = "Select Executable", 
            DefaultExt = ".exe", 
            Filter = "Executables (*.exe, *.bat)|*.exe;*.bat", 
            CheckFileExists = false
        )]
        public string ExecutablePath
        {
            get { return _executablePath; }
            set
            {
                if (string.Compare(_executablePath, value, StringComparison.Ordinal) != 0)
                {
                    _executablePath = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start arguments.
        /// </summary>
        [StringLength(2048)]
        [DisplayGroup("Executable")]
        [DisplayOrder(1)]
        [Description("The arguments to use when executing the executable.")]
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                if (string.Compare(_arguments, value, StringComparison.Ordinal) != 0)
                {
                    _arguments = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to run the executable elevated (as administrator).
        /// </summary>
        [DisplayGroup("Executable")]
        [DisplayOrder(3)]
        [Description("Run executable as administrator (elevated).")]
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
        /// Gets or sets whether to hide the process window for the executed executable.
        /// </summary>
        [DisplayGroup("Executable")]
        [DisplayOrder(4)]
        [DisplayName("Run Hidden")]
        [Description("Run executable in the background (no visible window).")]
        public bool HideProcessWindow
        {
            get { return _hideProcessWindow; }
            set
            {
                if (_hideProcessWindow != value)
                {
                    _hideProcessWindow = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}
