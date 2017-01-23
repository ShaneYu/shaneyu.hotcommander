using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Display Group
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DisplayGroupAttribute : Attribute
    {
        /// <summary>
        /// Gets the group name.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The group name</param>
        public DisplayGroupAttribute(string name)
        {
            Name = name;
        }
    }
}
