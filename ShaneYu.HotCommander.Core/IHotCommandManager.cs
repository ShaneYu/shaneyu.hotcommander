using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Searching;
using ShaneYu.HotCommander.Storage;

using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander
{
    /// <summary>
    /// Command Manager Interface
    /// </summary>
    public interface IHotCommandManager
    {
        event EventHandler<Guid> CommandDeleted;
        event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandLoaded;
        event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandSaved;
        event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandCreated;
        event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandRegistered;

        /// <summary>
        /// Gets the command storage strategy being used.
        /// </summary>
        ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>> StorageStrategy { get; }

        /// <summary>
        /// Register a command with the manager.
        /// Commands without an Id are presumed new and will be created.
        /// </summary>
        /// <typeparam name="T">Type of command settings used for the command being registered</typeparam>
        /// <param name="hotCommand">The command to be registered</param>
        void Register<T>(IHotCommand<T> hotCommand) where T : IHotCommandConfiguration;

        /// <summary>
        /// Get a specific command
        /// </summary>
        /// <typeparam name="T">Type of command to get</typeparam>
        /// <param name="id">Id of the command to get</param>
        /// <returns>Command with the specified Id or <c>null</c></returns>
        T Get<T>(Guid id) where T : IHotCommand<IHotCommandConfiguration>;

        /// <summary>
        /// Get a specific command
        /// </summary>
        /// <param name="id">Id of the command to get</param>
        /// <returns>Command with the specified Id or <c>null</c></returns>
        IHotCommand<IHotCommandConfiguration> Get(Guid id);

        /// <summary>
        /// Get all commands
        /// </summary>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the criteria.</returns>
        IEnumerable<IHotCommand<IHotCommandConfiguration>> GetAll(bool excludeInvariant = false, bool includeDisabled = false);

        /// <summary>
        /// Get all commands
        /// </summary>
        /// <typeparam name="T">Type of commands to get</typeparam>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the criteria.</returns>
        IEnumerable<T> GetAll<T>(bool excludeInvariant = false, bool includeDisabled = false) where T : IHotCommand<IHotCommandConfiguration>;

        /// <summary>
        /// Search all commands.
        /// Uses the default command search strategy: <see cref="T:HotCommander.Core.Implementations.DefaultCommandSearchStrategy"/>
        /// </summary>
        /// <param name="searchTerm">The term to search.</param>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the search criteria.</returns>
        IEnumerable<IHotCommand<IHotCommandConfiguration>> Search(string searchTerm, bool excludeInvariant = false, bool includeDisabled = false);

        /// <summary>
        /// Search all commands.
        /// Using a custom command search strategy specified in the <paramref name="searchStrategy"/> parameter.
        /// </summary>
        /// <typeparam name="T">Type of commands to search</typeparam>
        /// <param name="searchStrategy">The command search strategy to use.</param>
        /// <param name="searchTerm">The term to search.</param>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the search criteria.</returns>
        IEnumerable<T> Search<T>(ISearchStrategy<T> searchStrategy, string searchTerm, bool excludeInvariant = false, bool includeDisabled = false);

        /// <summary>
        /// Create a new command and registers it with the manager.
        /// </summary>
        /// <typeparam name="T">Type of command being created.</typeparam>
        /// <param name="command">The command being created.</param>
        /// <returns><c>true</c> if command was successfully created, otherwise <c>false</c></returns>
        Task<bool> CreateAsync<T>(T command) where T : IHotCommand<IHotCommandConfiguration>;

        /// <summary>
        /// Delete all existing commands
        /// </summary>
        Task<bool> DeleteAsync();

        /// <summary>
        /// Delete an existing command
        /// </summary>
        /// <param name="id"></param>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Save all commands (async)
        /// </summary>
        Task<bool> SaveAsync();

        /// <summary>
        /// Save a specific command
        /// </summary>
        /// <param name="id"></param>
        Task<bool> SaveAsync(Guid id);

        /// <summary>
        /// Load all commands (async)
        /// </summary>
        /// <returns></returns>
        Task LoadAsync();

        /// <summary>
        /// Reload all commands (async)
        /// </summary>
        Task ReloadAsync();

        /// <summary>
        /// Reload a specific command
        /// </summary>
        /// <param name="id"></param>
        Task<bool> ReloadAsync(Guid id);
    }
}
