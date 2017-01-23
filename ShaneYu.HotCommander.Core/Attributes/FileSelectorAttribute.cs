using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// File Selector Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FileSelectorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the file selector dialog title.
        /// </summary>
        public virtual string Title { get; set; } = "File Selector";

        /// <summary>
        /// Gets or sets the default extension (without '.').
        /// </summary>
        public virtual string DefaultExt { get; set; }

        /// <summary>
        /// Gets or sets the file filter.
        /// </summary>
        public virtual string Filter { get; set; } = "All Files (*.*)|*.*";

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        public virtual string InitialDirectory { get; set; } = "%SystemDrive%";

        /// <summary>
        /// Gets or sets whether to check if the desired filename already exists.
        /// </summary>
        public virtual bool CheckFileExists { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to restore the previous directory if the dialog is opened again.
        /// </summary>
        public virtual bool RestoreDirectory { get; set; } = true;
    }
}
