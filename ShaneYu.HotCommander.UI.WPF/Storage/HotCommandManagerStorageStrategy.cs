using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Extensions;
using ShaneYu.HotCommander.Logging;
using ShaneYu.HotCommander.Storage;

using Newtonsoft.Json;

namespace ShaneYu.HotCommander.UI.WPF.Storage
{
    public class HotCommandManagerStorageStrategy : ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>>
    {
        #region NestedTypes

        private struct LoadCommandResult
        {
            public IHotCommand<IHotCommandConfiguration> Command { get; set; }
            public StorageFailureDetail FailureDetail { get; set; }
        }

        #endregion

        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public HotCommandManagerStorageStrategy(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Private Methods

        private static Guid? TryParseGuid(string value)
        {
            Guid outGuid;

            if (Guid.TryParse(value, out outGuid))
            {
                return outGuid;
            }

            return null;
        }

        private LoadCommandResult LoadCommand(Guid id)
        {
            var filePath = Environment.ExpandEnvironmentVariables($"{App.Current.DataDirectory}\\Commands\\{id}.json");

            if (!File.Exists(filePath))
            {
                return new LoadCommandResult
                {
                    FailureDetail =
                        new StorageFailureDetail("The specified command ID does not exist.",
                            new FileNotFoundException("The command file could not be found.", filePath))
                };
            }

            string cmdJson;

            try
            {
                cmdJson = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                return new LoadCommandResult
                {
                    FailureDetail =
                        new StorageFailureDetail("Unable to read command file. See exception for more details.", ex)
                };
            }

            try
            {
                var cmd = (IHotCommand<IHotCommandConfiguration>)JsonConvert.DeserializeObject(cmdJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, MissingMemberHandling = MissingMemberHandling.Ignore });

                return new LoadCommandResult { Command = cmd };
            }
            catch (Exception ex)
            {
                return new LoadCommandResult
                {
                    FailureDetail =
                        new StorageFailureDetail(
                            "Unable to instantiate the command. See exception for more details.", ex)
                };
            }
        }

        private StorageFailureDetail SaveCommand(IHotCommand<IHotCommandConfiguration> command)
        {
            if (command.IsInternal())
            {
                return new StorageFailureDetail(
                    "Saving of an invariant command is not allowed.");
            }

            var commandDirectory = Environment.ExpandEnvironmentVariables($"{App.Current.DataDirectory}\\Commands");

            if (!Directory.Exists(commandDirectory))
            {
                try
                {
                    Directory.CreateDirectory(commandDirectory);
                }
                catch (Exception ex)
                {
                    return new StorageFailureDetail(
                        "Unable to create the directory where the command should be saved. See exception for more details.",
                        ex);
                }
            }

            var filePath = $"{commandDirectory}\\{command.Configuration.Id}.json";
            var jsonString = JsonConvert.SerializeObject(command, Formatting.Indented,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            try
            {
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                return new StorageFailureDetail(
                    "Unable to write command file. See exception for more details.", ex);
            }

            if (!File.Exists(filePath))
            {
                return new StorageFailureDetail(
                    "Unable to save command; file doesn't exist after performing save operation.");
            }

            return null;
        }

        private StorageFailureDetail DeleteCommand(IHotCommand<IHotCommandConfiguration> command)
        {
            if (command.IsInternal())
            {
                return new StorageFailureDetail("Deleting of an invariant command is not allowed.");
            }

            var filePath = Environment.ExpandEnvironmentVariables($"{App.Current.DataDirectory}\\Commands\\{command.Configuration.Id}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    return new StorageFailureDetail(
                        "Unable to delete the command file.See exception for more details.", ex);
                }

                if (File.Exists(filePath))
                {
                    return new StorageFailureDetail(
                        "Unable to delete command file. The file persists after performing delete operation.");
                }
            }

            return null;
        }

        #endregion

        #region Public Methods

        public Task<StorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>> LoadAsync(Guid id)
        {
            var taskCompletionSource =
                new TaskCompletionSource<StorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>>();

            var result = LoadCommand(id);

            if (result.Command != null)
            {
                taskCompletionSource.SetResult(
                    new StorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>(id, result.Command));
            }
            else
            {
                _logger.Warn(result.FailureDetail.Exception, "Command ID: {0}\n\n{1}", id, result.FailureDetail.Reason);

                taskCompletionSource.SetResult(
                    new StorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>(id, result.FailureDetail));
            }

            return taskCompletionSource.Task;
        }

        public Task<StorageSaveResult<Guid, IHotCommand<IHotCommandConfiguration>>> SaveAsync(Guid id, IHotCommand<IHotCommandConfiguration> command)
        {
            var taskCompletionSource =
                new TaskCompletionSource<StorageSaveResult<Guid, IHotCommand<IHotCommandConfiguration>>>();

            var failure = SaveCommand(command);

            if (failure != null)
            {
                _logger.Warn(failure.Exception, "Command ID: {0}\n\n{1}", id, failure.Reason);
                taskCompletionSource.SetResult(new StorageSaveResult<Guid, IHotCommand<IHotCommandConfiguration>>(id, command, failure));
            }
            else
            {
                taskCompletionSource.SetResult(new StorageSaveResult<Guid, IHotCommand<IHotCommandConfiguration>>(id, command));
            }

            return taskCompletionSource.Task;
        }

        public Task<StorageDeleteResult<Guid>> DeleteAsync(Guid id, IHotCommand<IHotCommandConfiguration> command)
        {
            var taskCompletionSource =
                new TaskCompletionSource<StorageDeleteResult<Guid>>();


            var failure = DeleteCommand(command);

            if (failure != null)
            {
                _logger.Warn(failure.Exception, "Command ID: {0}\n\n{1}", id, failure.Reason);
                taskCompletionSource.SetResult(new StorageDeleteResult<Guid>(id, failure));
            }
            else
            {
                taskCompletionSource.SetResult(new StorageDeleteResult<Guid>(id));
            }

            return taskCompletionSource.Task;
        }

        public async Task<IEnumerable<IHotCommand<IHotCommandConfiguration>>> LoadAllAsync()
        {
            var commandDirectory = Environment.ExpandEnvironmentVariables($"{App.Current.DataDirectory}\\Commands");

            if (!Directory.Exists(commandDirectory))
            {
                return Enumerable.Empty<IHotCommand<IHotCommandConfiguration>>();
            }

            var cmdIds = from filePath in Directory.GetFiles(commandDirectory, "*.json", SearchOption.TopDirectoryOnly)
                         let strGuid = Path.GetFileNameWithoutExtension(filePath)
                         where !string.IsNullOrWhiteSpace(strGuid)
                         let guid = TryParseGuid(strGuid)
                         where guid.HasValue
                         select guid.Value;

            return (await LoadAsync(cmdIds)).Items;
        }

        public Task<CollectionStorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>> LoadAsync(IEnumerable<Guid> ids)
        {
            var taskCompletionSource = new TaskCompletionSource<CollectionStorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>>();
            var cmdIds = ids.ToArray();
            var cmdTasks = cmdIds.Select(LoadAsync).ToArray();

            Task.WaitAll(cmdTasks.Cast<Task>().ToArray());

            var loadedCommands =
                cmdTasks.Where(x => x.IsCompleted && !x.IsCanceled && !x.IsFaulted && x.Result.Success)
                    .Select(x => x.Result.Item);

            taskCompletionSource.SetResult(new CollectionStorageLoadResult<Guid, IHotCommand<IHotCommandConfiguration>>(cmdIds, loadedCommands));

            return taskCompletionSource.Task;
        }

        public Task<CollectionStorageSaveResult<Guid, IHotCommand<IHotCommandConfiguration>>> SaveAsync(IEnumerable<KeyValuePair<Guid, IHotCommand<IHotCommandConfiguration>>> commands)
        {
            throw new NotImplementedException();
        }

        public Task<CollectionStorageDeleteResult<Guid>> DeleteAsync(IEnumerable<KeyValuePair<Guid, IHotCommand<IHotCommandConfiguration>>> commands)
        {
            throw new NotImplementedException();
        }

        public Task<CollectionStorageDeleteResult<Guid>> DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
