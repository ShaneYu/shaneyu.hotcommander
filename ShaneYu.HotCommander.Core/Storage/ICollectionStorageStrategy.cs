using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShaneYu.HotCommander.Storage
{
    /// <summary>
    /// Collection Storage Strategy Interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used by the storage strategy.</typeparam>
    /// <typeparam name="TItem">The type of item used by the storage strategy.</typeparam>
    public interface ICollectionStorageStrategy<TKey, TItem> : IStorageStrategy<TKey, TItem> where TItem : class
    {
        /// <summary>
        /// Load all items from storage
        /// </summary>
        /// <returns>The collection storage load result</returns>
        Task<IEnumerable<TItem>> LoadAllAsync();

        /// <summary>
        /// Load multiple items from storage
        /// </summary>
        /// <param name="keys">Keys of all the item to load</param>
        /// <returns>The collection storage load result</returns>
        Task<CollectionStorageLoadResult<TKey, TItem>> LoadAsync(IEnumerable<TKey> keys);

        /// <summary>
        /// Save multiple items to storage
        /// </summary>
        /// <param name="items">The items to save</param>
        /// <returns>The collection storage save result</returns>
        Task<CollectionStorageSaveResult<TKey, TItem>> SaveAsync(IEnumerable<KeyValuePair<TKey, TItem>> items);

        /// <summary>
        /// Delete multiple items from storage
        /// </summary>
        /// <param name="items">The items to delete</param>
        /// <returns>The collection storage delete result</returns>
        Task<CollectionStorageDeleteResult<TKey>> DeleteAsync(IEnumerable<KeyValuePair<TKey, TItem>> items);

        /// <summary>
        /// Delete all items from storage
        /// </summary>
        /// <returns>The collection storage delete result</returns>
        Task<CollectionStorageDeleteResult<TKey>> DeleteAllAsync();
    }
}
