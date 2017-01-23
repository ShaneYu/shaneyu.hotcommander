using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Display Group Order
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DisplayGroupOrderAttribute : Attribute
    {
        /// <summary>
        /// Gets the group name.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets the group order.
        /// </summary>
        public virtual int Order { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The group name to set the order for.</param>
        /// <param name="order">The order to set for the group</param>
        public DisplayGroupOrderAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }
}
