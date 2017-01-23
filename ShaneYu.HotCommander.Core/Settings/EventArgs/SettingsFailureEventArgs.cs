using System;

namespace ShaneYu.HotCommander.Settings.EventArgs
{
    /// <summary>
    /// Settings Failure Event Args
    /// </summary>
    public class SettingsFailureEventArgs : System.EventArgs
    {
        #region Properties

        /// <summary>
        /// The reason, if any, why the settings operation failed.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// The exception, if any, that was thrown when the settings operation failed.
        /// </summary>
        public Exception Exception { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reason">The reaon the settings operation failed.</param>
        public SettingsFailureEventArgs(string reason)
        {
            Reason = reason;
            Exception = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">The exception thrown when the settings operation failed.</param>
        public SettingsFailureEventArgs(Exception exception)
        {
            Reason = "An error occurred. See the exception for more details.";
            Exception = exception;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reason">The reaon the settings operation failed.</param>
        /// <param name="exception">The exception thrown when the settings operation failed.</param>
        public SettingsFailureEventArgs(string reason, Exception exception)
        {
            Reason = reason;
            Exception = exception;
        }

        #endregion
    }
}
