using ShaneYu.HotCommander.Storage;

namespace ShaneYu.HotCommander.Settings
{
    /// <summary>
    /// Settings Result
    /// </summary>
    public class SettingsResult
    {
        /// <summary>
        /// Gets whether the settings operation was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the failure details, if settings operation failed.
        /// </summary>
        public StorageFailureDetail FailureDetail { get; }

        /// <summary>
        /// Constructor (Successful)
        /// </summary>
        public SettingsResult()
        {
            Success = true;
            FailureDetail = null;
        }

        /// <summary>
        /// Constructor (Failure)
        /// </summary>
        /// <param name="failureDetail">The storage failure details</param>
        public SettingsResult(StorageFailureDetail failureDetail)
        {
            Success = false;
            FailureDetail = failureDetail;
        }
    }
}
