using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ShaneYu.HotCommander.Attributes;

namespace ShaneYu.HotCommander.Commands.Alias
{
    /// <summary>
    /// Alias Configuration Interface
    /// </summary>
    public interface IAliasConfiguration : IHotCommandConfiguration
    {
        /// <summary>
        /// Gets or sets the target command id.
        /// </summary>
        Guid TargetCommandId { get; }
    }

    /// <summary>
    /// Alias Configuration
    /// </summary>
    [Serializable]
    [DisplayGroupOrder("Core", 0)]
    [DisplayGroupOrder("Alias", 1)]
    public class AliasConfiguration : HotCommandConfigurationBase, IAliasConfiguration
    {
        #region Fields

        private Guid _id;
        private string _name;
        private string _description;
        private bool _isEnabled = true;

        private Guid _targetCommandId;

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
        /// Gets or sets the target command id.
        /// </summary>
        [Required]
        [DisplayOrder(0)]
        [DisplayGroup("Alias")]
        [Description("ID of the target command to execute when this alias command id executed.")]
        [UIHint("CommandSelector")]
        public Guid TargetCommandId
        {
            get { return _targetCommandId; }
            set
            {
                if (!Equals(_targetCommandId, value))
                {
                    _targetCommandId = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}
