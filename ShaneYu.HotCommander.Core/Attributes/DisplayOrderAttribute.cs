using System;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Display Order
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DisplayOrderAttribute : Attribute
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        public virtual int Order { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="order">The order</param>
        public DisplayOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
