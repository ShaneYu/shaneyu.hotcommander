using System;
using System.ComponentModel;

namespace ShaneYu.HotCommander.Commands
{
    /// <summary>
    /// Command Configuration Interface
    /// </summary>
    public interface IHotCommandConfiguration : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or set the command ID.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the command description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets whether the command is enabled.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
