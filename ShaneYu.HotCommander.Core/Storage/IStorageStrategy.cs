using System.Threading.Tasks;

namespace ShaneYu.HotCommander.Storage
{
    /// <summary>
    /// Storage Strategy Interface
    /// </summary>
    /// <typeparam name="TKey">The type of key used by the storage strategy.</typeparam>
    /// <typeparam name="TItem">The type of item used by the storage strategy.</typeparam>
    public interface IStorageStrategy<TKey, TItem> where TItem : class
    {
        /// <summary>
        /// Load an item from storage.
        /// </summary>
        /// <param name="key">Key of item to load.</param>
        /// <returns>The storage load result.</returns>
        Task<StorageLoadResult<TKey, TItem>> LoadAsync(TKey key);

        /// <summary>
        /// Save an item to storage.
        /// </summary>
        /// <param name="key">Key of the item to save.</param>
        /// <param name="item">The item to save.</param>
        /// <returns>The storage save result.</returns>
        Task<StorageSaveResult<TKey, TItem>> SaveAsync(TKey key, TItem item);

        /// <summary>
        /// Delete an item from storage.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <param name="item">The item to delete.</param>
        /// <returns>The storage delete result.</returns>
        Task<StorageDeleteResult<TKey>> DeleteAsync(TKey key, TItem item);
    }
}
