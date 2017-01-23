using System;

namespace ShaneYu.HotCommander.UI.WPF.Logging
{
    /// <summary>
    /// Logger Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class)]
    public class LoggerAttribute : Attribute
    {
        #region Properties

        public readonly string Name;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Desired logger name</param>
        public LoggerAttribute(string name)
        {
            Name = name;
        }

        #endregion
    }
}
