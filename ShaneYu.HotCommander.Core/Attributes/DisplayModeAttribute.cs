using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Display Mode
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayModeAttribute : Attribute
    {
        /// <summary>
        /// Gets the display mode when creating a new instance.
        /// </summary>
        public virtual DisplayMode CreateMode { get; }

        /// <summary>
        /// Gets the display mode when editing an existing instance.
        /// </summary>
        public virtual DisplayMode EditMode { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="createMode">The display mode when creating a new instance</param>
        /// <param name="editMode">The display mode when editing an existing instance</param>
        public DisplayModeAttribute(DisplayMode createMode = DisplayMode.Editable, DisplayMode editMode = DisplayMode.Editable)
        {
            CreateMode = createMode;
            EditMode = editMode;
        }
    }

    /// <summary>
    /// Display Mode
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Should not be visible at all.
        /// </summary>
        Hidden,

        /// <summary>
        /// Should be visible, but reaonly.
        /// </summary>
        Readonly,

        /// <summary>
        /// Should be visible and editable.
        /// </summary>
        Editable
    }
}
