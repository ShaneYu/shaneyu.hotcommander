using System;

namespace ShaneYu.HotCommander.Storage
{
    /// <summary>
    /// Storage Failure Detail
    /// </summary>
    public class StorageFailureDetail
    {
        #region Properties

        /// <summary>
        /// Gets the reason for the failure.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the exception thrown during the failure.
        /// </summary>
        public Exception Exception { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new storage failure detail with a reason.
        /// </summary>
        /// <param name="reason">The reason for the failure.</param>
        public StorageFailureDetail(string reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// Instantiates a new storage failure detail with an exception.
        /// </summary>
        /// <param name="exception">The exception thrown during the failure.</param>
        public StorageFailureDetail(Exception exception)
        {
            Reason = "An unexpected error occurred, please see the exception for more detail.";
            Exception = exception;
        }

        /// <summary>
        /// Instantiates a new storage failure detail with a reason and an exception.
        /// </summary>
        /// <param name="reason">The reason for the failure.</param>
        /// <param name="exception">The exception thrown during the failure.</param>
        public StorageFailureDetail(string reason, Exception exception)
        {
            Reason = reason;
            Exception = exception;
        }

        #endregion
    }

    /// <summary>
    /// Storage Failure Detail (with item data)
    /// </summary>
    /// <typeparam name="T">Type of item data.</typeparam>
    public class StorageFailureDetail<T> : StorageFailureDetail
    {
        #region Properties

        /// <summary>
        /// Gets the item which failed.
        /// </summary>
        public T Item { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new storage failure detail with a reason.
        /// </summary>
        /// <param name="item">The item which failed.</param>
        /// <param name="reason">The reason for the failure.</param>
        public StorageFailureDetail(T item, string reason)
            : base(reason)
        {
            Item = item;
        }

        /// <summary>
        /// Instantiates a new storage failure detail with an exception.
        /// </summary>
        /// <param name="item">The item which failed.</param>
        /// <param name="exception">The exception thrown during the failure.</param>
        public StorageFailureDetail(T item, Exception exception)
            : base(exception)
        {
            Item = item;
        }

        /// <summary>
        /// Instantiates a new storage failure detail with a reason and an exception.
        /// </summary>
        /// <param name="item">The item which failed.</param>
        /// <param name="reason">The reason for the failure.</param>
        /// <param name="exception">The exception thrown during the failure.</param>
        public StorageFailureDetail(T item, string reason, Exception exception)
            : base(reason, exception)
        {
            Item = item;
        }

        #endregion
    }
}
