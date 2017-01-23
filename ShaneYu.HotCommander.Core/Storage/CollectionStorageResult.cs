using System.Collections.Generic;
using System.Linq;

namespace ShaneYu.HotCommander.Storage
{
    /// <summary>
    /// Collection Storage Result
    /// </summary>
    public abstract class CollectionStorageResult<TKey>
    {
        #region Properties

        /// <summary>
        /// Gets the storage failure detail if unsuccessful, otherwise <c>null</c>.
        /// </summary>
        public IEnumerable<StorageFailureDetail<TKey>> FailureDetails { get; }

        /// <summary>
        /// Gets whether or not the storage operation was successful.
        /// </summary>
        public bool Success { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new collection successful storage result.
        /// </summary>
        public CollectionStorageResult()
        {
            FailureDetails = null;
            Success = true;
        }

        /// <summary>
        /// Instantiates a new collection failure storage result.
        /// </summary>
        /// <param name="failureDetails">The failure details.</param>
        public CollectionStorageResult(IEnumerable<StorageFailureDetail<TKey>> failureDetails)
        {
            FailureDetails = failureDetails;
            Success = (FailureDetails == null || !FailureDetails.Any());
        }

        #endregion
    }

    /// <summary>
    /// Collection Storage Load Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    public class CollectionStorageLoadResult<TKey, TItem> : CollectionStorageResult<TKey>
    {
        #region Properties

        /// <summary>
        /// Gets the keys of all items that were to be loaded.
        /// </summary>
        public IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets all of the successfully loaded items; otherwise <c>null</c>.
        /// </summary>
        public IEnumerable<TItem> Items { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new collection storage load result.
        /// </summary>
        /// <param name="keys">The keys of all items that were to be loaded.</param>
        /// <param name="items">The items which were loaded successfully.</param>
        /// <param name="failureDetails">The failure details for all the items which failed to load.</param>
        public CollectionStorageLoadResult(IEnumerable<TKey> keys, IEnumerable<TItem> items, IEnumerable<StorageFailureDetail<TKey>> failureDetails = null)
            : base(failureDetails)
        {
            Keys = keys;
            Items = items;
        }

        #endregion
    }

    /// <summary>
    /// Collection Storage Save Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    public class CollectionStorageSaveResult<TKey, TItem> : CollectionStorageResult<TKey>
    {
        #region Properties

        /// <summary>
        /// Gets the keys of all items that were to be saved.
        /// </summary>
        public IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets all of the successfully saved items; otherwise <c>null</c>.
        /// </summary>
        public IEnumerable<TItem> Items { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new collection storage save result.
        /// </summary>
        /// <param name="keys">The keys of all items that were to be saved.</param>
        /// <param name="items">The items which were saved successfully.</param>
        /// <param name="failureDetails">The failure details for all the items which failed to save.</param>
        public CollectionStorageSaveResult(IEnumerable<TKey> keys, IEnumerable<TItem> items, IEnumerable<StorageFailureDetail<TKey>> failureDetails = null)
            : base(failureDetails)
        {
            Keys = keys;
            Items = items;
        }

        #endregion
    }

    /// <summary>
    /// Collection Storage Delete Result
    /// </summary>
    /// <typeparam name="TKey">Type of item key</typeparam>
    public class CollectionStorageDeleteResult<TKey> : CollectionStorageResult<TKey>
    {
        #region Properties

        /// <summary>
        /// Gets the keys of all items that were to be deleted.
        /// </summary>
        public IEnumerable<TKey> Keys { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new collection storage delete result.
        /// </summary>
        /// <param name="keys">The keys of all items that were to be deleted.</param>
        /// <param name="failureDetails">The failure details for all the items which failed to delete.</param>
        public CollectionStorageDeleteResult(IEnumerable<TKey> keys, IEnumerable<StorageFailureDetail<TKey>> failureDetails = null)
            : base(failureDetails)
        {
            Keys = keys;
        }

        #endregion
    }
}
