using System;
namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Folder Selector Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FolderSelectorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the folder dialog description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the folder dialog root folder.
        /// </summary>
        public virtual Environment.SpecialFolder RootFolder { get; set; } = Environment.SpecialFolder.MyComputer;

        /// <summary>
        /// Gets or sets whether the new folder button is shown in the dialog.
        /// </summary>
        public virtual bool ShowNewFolderButton { get; set; } = true;
    }
}
