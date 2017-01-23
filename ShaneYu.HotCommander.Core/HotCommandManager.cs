using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Searching;
using ShaneYu.HotCommander.Storage;

namespace ShaneYu.HotCommander
{
    /// <summary>
    /// Command Manager
    /// </summary>
    public class HotCommandManager : IHotCommandManager
    {
        #region Fields

        private readonly ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>> _storageStrategy;
        private readonly ISearchStrategy<IHotCommand<IHotCommandConfiguration>> _searchStrategy;
        private readonly List<IHotCommand<IHotCommandConfiguration>> _registeredCommands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command storage strategy being used.
        /// </summary>
        public ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>> StorageStrategy => _storageStrategy;

        #endregion

        #region Events

        public event EventHandler<Guid> CommandDeleted;
        public event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandLoaded;
        public event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandSaved;
        public event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandCreated;
        public event EventHandler<IHotCommand<IHotCommandConfiguration>> CommandRegistered;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageStrategy">The storage strategy to use</param>
        public HotCommandManager(ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>> storageStrategy)
        {
            _storageStrategy = storageStrategy;
            _searchStrategy = new DefaultSearchStrategy();
            _registeredCommands = new List<IHotCommand<IHotCommandConfiguration>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a command with the manager.
        /// Commands without an Id are presumed new and will be created.
        /// </summary>
        /// <typeparam name="T">Type of command settings used for the command being registered</typeparam>
        /// <param name="hotCommand">The command to be registered</param>
        public void Register<T>(IHotCommand<T> hotCommand) where T : IHotCommandConfiguration
        {
            var baseCommand = (IHotCommand<IHotCommandConfiguration>)hotCommand;

            if (!_registeredCommands.Contains(baseCommand))
            {
                _registeredCommands.Add(baseCommand);
                CommandRegistered?.Invoke(this, baseCommand);
            }
        }

        /// <summary>
        /// Get a specific command
        /// </summary>
        /// <typeparam name="T">Type of command to get</typeparam>
        /// <param name="id">Id of the command to get</param>
        /// <returns>Command with the specified Id or <c>null</c></returns>
        public T Get<T>(Guid id) where T : IHotCommand<IHotCommandConfiguration>
        {
            return (T)Get(id);
        }

        /// <summary>
        /// Get a specific command
        /// </summary>
        /// <param name="id">Id of the command to get</param>
        /// <returns>Command with the specified Id or <c>null</c></returns>
        public IHotCommand<IHotCommandConfiguration> Get(Guid id)
        {
            return _registeredCommands.FirstOrDefault(x => x.Configuration.Id == id);
        }

        /// <summary>
        /// Get all commands
        /// </summary>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the criteria.</returns>
        public IEnumerable<IHotCommand<IHotCommandConfiguration>> GetAll(bool excludeInvariant = false, bool includeDisabled = false)
        {
            return
                _registeredCommands.Where(
                    x =>
                        (includeDisabled || x.Configuration.IsEnabled) &&
                        (!excludeInvariant || !x.GetType().GetCustomAttributes(typeof(InternalHotCommandAttribute), false).Any()));
        }

        /// <summary>
        /// Get all commands
        /// </summary>
        /// <typeparam name="T">Type of commands to get</typeparam>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the criteria.</returns>
        public IEnumerable<T> GetAll<T>(bool excludeInvariant = false, bool includeDisabled = false) where T : IHotCommand<IHotCommandConfiguration>
        {
            return GetAll(excludeInvariant, includeDisabled).OfType<T>();
        }

        /// <summary>
        /// Search all commands.
        /// Uses the default command search strategy: <see cref="T:HotCommander.Searching.DefaultSearchStrategy"/>
        /// </summary>
        /// <param name="searchTerm">The term to search.</param>
        /// <param name="excludeInvariant">Whether to exclude invariant commands.</param>
        /// <param name="includeDisabled">Whether to include disabled commands.</param>
        /// <returns>All commands matching the search criteria.</returns>
        public IEnumerable<IHotCommand<IHotCommandConfiguration>> Search(string searchTerm, bool excludeInvariant = false, bool includeDisabled = false)
        {
            return Search(_searchStrategy, searchTerm, excludeInvariant, includeDisabled);
        }

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
        public IEnumerable<T> Search<T>(ISearchStrategy<T> searchStrategy, string searchTerm, bool excludeInvariant = false, bool includeDisabled = false)
        {
            return searchStrategy.Search(GetAll(), searchTerm, excludeInvariant, includeDisabled);
        }

        /// <summary>
        /// Create a new command and registers it with the manager.
        /// </summary>
        /// <typeparam name="T">Type of command being created.</typeparam>
        /// <param name="command">The command being created.</param>
        /// <returns><c>true</c> if command was successfully created, otherwise <c>false</c></returns>
        public async Task<bool> CreateAsync<T>(T command) where T : IHotCommand<IHotCommandConfiguration>
        {
            command.Configuration.Id = Guid.NewGuid();

            // TODO: If save failed, it would be nice to use the IStorageResult and pass back the reason that it failed.
            if ((await _storageStrategy.SaveAsync(command.Configuration.Id, command)).Success)
            {
                Register(command);
                CommandCreated?.Invoke(this, command);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete all existing commands (async)
        /// </summary>
        public async Task<bool> DeleteAsync()
        {
            var result = await _storageStrategy.DeleteAllAsync();

            if (result.Success)
            {
                // Since all commands deleted successfully, remove them all.
                foreach (var cmd in _registeredCommands.ToArray())
                {
                    _registeredCommands.Remove(cmd);
                    CommandDeleted?.Invoke(this, cmd.Configuration.Id);
                }

                return true;
            }

            // Only remove the commands that were successfully deleted.
            foreach (var cmd in _registeredCommands.ToArray().Where(x => result.FailureDetails.All(y => y.Item != x.Configuration.Id)))
            {
                _registeredCommands.Remove(cmd);
                CommandDeleted?.Invoke(this, cmd.Configuration.Id);
            }

            // TODO: If delete failed, it would be nice to use the CollectionStorageResult and pass back the commands that failed delete.
            return false;
        }

        /// <summary>
        /// Delete an existing command (async)
        /// </summary>
        /// <param name="id"></param>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var command = Get(id);

            if (command == null)
            {
                // Command doesn't exist, so no need to report failure for not deleting it; just return success.
                return true;
            }

            // TODO: If delete failed, it would be nice to use the CollectionStorageResult and pass back the reason the delete failed.
            if ((await _storageStrategy.DeleteAsync(command.Configuration.Id, command)).Success)
            {
                _registeredCommands.Remove(command);
                CommandDeleted?.Invoke(this, command.Configuration.Id);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Save all commands (async)
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            // TODO: If save all failed, it would be nice to use the CollectionStorageResult and pass back the commands that failed to save.
            var commands = GetAll().Select(x => new KeyValuePair<Guid, IHotCommand<IHotCommandConfiguration>>(x.Configuration.Id, x)).ToArray();
            var result = await _storageStrategy.SaveAsync(commands);

            if (result.Success)
            {
                foreach (var cmd in commands)
                {
                    CommandSaved?.Invoke(this, cmd.Value);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Save a specific command (async)
        /// </summary>
        /// <param name="id"></param>
        public async Task<bool> SaveAsync(Guid id)
        {
            var command = Get(id);

            if (command != null)
            {
                // TODO: If save failed, it would be nice to use the CollectionStorageResult and pass back the reason the save failed.
                var result = await _storageStrategy.SaveAsync(command.Configuration.Id, command);

                if (result.Success)
                {
                    CommandSaved?.Invoke(this, result.Item);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Load all commands (async)
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            var loadedCommands = await _storageStrategy.LoadAllAsync();

            foreach (var cmd in loadedCommands)
            {
                _registeredCommands.Add(cmd);
                CommandLoaded?.Invoke(this, cmd);
            }
        }

        /// <summary>
        /// Reload all commands (async)
        /// </summary>
        public async Task ReloadAsync()
        {
            foreach (var cmd in GetAll(excludeInvariant: true, includeDisabled: true).ToArray())
            {
                _registeredCommands.Remove(cmd);
            }

            await LoadAsync();
        }

        /// <summary>
        /// Reload a specific command (async)
        /// </summary>
        /// <param name="id"></param>
        public async Task<bool> ReloadAsync(Guid id)
        {
            var command = Get(id);

            if (command != null)
            {
                _registeredCommands.Remove(command);
            }

            var result = await _storageStrategy.LoadAsync(id);

            if (result.Success && result.Item != null)
            {
                _registeredCommands.Add(result.Item);
                CommandLoaded?.Invoke(this, result.Item);
                return true;
            }

            return false;
        }

        #endregion
    }
}
 