namespace ShaneYu.HotCommander.Storage
{
    /// <summary>
    /// Storage Result
    /// </summary>
    public abstract class StorageResult
    {
        #region Properties

        /// <summary>
        /// Gets the storage failure detail if unsuccessful, otherwise <c>null</c>.
        /// </summary>
        public StorageFailureDetail FailureDetail { get; }

        /// <summary>
        /// Gets whether or not the storage operation was successful.
        /// </summary>
        public bool Success { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new successful storage result.
        /// </summary>
        public StorageResult()
        {
            FailureDetail = null;
            Success = true;
        }

        /// <summary>
        /// Instantiates a new failure storage result.
        /// </summary>
        /// <param name="failureDetail">The failure details.</param>
        public StorageResult(StorageFailureDetail failureDetail)
        {
            FailureDetail = failureDetail;
            Success = (FailureDetail == null);
        }

        #endregion
    }

    /// <summary>
    /// Storage Load Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    public class StorageLoadResult<TKey, TItem> : StorageResult
    {
        #region Properties

        /// <summary>
        /// Gets the key of the item that was meant to be loaded.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the loaded item, if loading was successful; otherwise <c>null</c>.
        /// </summary>
        public TItem Item { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new successful storage load result.
        /// </summary>
        /// <param name="key">The key of the item which was loaded.</param>
        /// <param name="item">The item which was loaded.</param>
        public StorageLoadResult(TKey key, TItem item)
        {
            Key = key;
            Item = item;
        }

        /// <summary>
        /// Instantiates a new failure storage load result.
        /// </summary>
        /// <param name="key">The key of the item which failed to be loaded.</param>
        /// <param name="failureDetail">The load failure details.</param>
        public StorageLoadResult(TKey key, StorageFailureDetail failureDetail)
            : base(failureDetail)
        {
            Key = key;
        }

        #endregion
    }

    /// <summary>
    /// Storage Save Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    public class StorageSaveResult<TKey, TItem> : StorageResult
    {
        #region Properties

        /// <summary>
        /// Gets the key of the item that was meant to be saved.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the item that was being saved.
        /// </summary>
        public TItem Item { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new successful storage save result.
        /// </summary>
        /// <param name="key">The key of the item which was saved.</param>
        /// <param name="item">The item which was saveed.</param>
        public StorageSaveResult(TKey key, TItem item)
        {
            Key = key;
            Item = item;
        }

        /// <summary>
        /// Instantiates a new failure storage save result.
        /// </summary>
        /// <param name="key">The key of the item which failed to be saved.</param>
        /// <param name="item">The item which was saveed.</param>
        /// <param name="failureDetail">The save failure details.</param>
        public StorageSaveResult(TKey key, TItem item, StorageFailureDetail failureDetail)
            : base(failureDetail)
        {
            Key = key;
            Item = item;
        }

        #endregion
    }

    /// <summary>
    /// Storage Delete Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    public class StorageDeleteResult<TKey> : StorageResult
    {
        #region Properties

        /// <summary>
        /// Gets the key of the item that was meant to be deleted.
        /// </summary>
        public TKey Key { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new successful storage delete result.
        /// </summary>
        /// <param name="key">The key of the item which was deleted.</param>
        public StorageDeleteResult(TKey key)
        {
            Key = key;
        }

        /// <summary>
        /// Instantiates a new failure storage delete result.
        /// </summary>
        /// <param name="key">The key of the item which failed to be deleted.</param>
        /// <param name="failureDetail">The delete failure details.</param>
        public StorageDeleteResult(TKey key, StorageFailureDetail failureDetail)
            : base(failureDetail)
        {
            Key = key;
        }

        #endregion
    }
}
