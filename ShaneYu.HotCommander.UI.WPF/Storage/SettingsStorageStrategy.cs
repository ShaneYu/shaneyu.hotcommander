using System;
using System.IO;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Logging;
using ShaneYu.HotCommander.Storage;

using Newtonsoft.Json;

namespace ShaneYu.HotCommander.UI.WPF.Storage
{
    /// <summary>
    /// Settings Storage Strategy
    /// </summary>
    /// <typeparam name="TItem">Type of settings being handled by this strategy implementation</typeparam>
    public class SettingsStorageStrategy<TItem> : IStorageStrategy<string, TItem>
        where TItem : class
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Properties

        protected virtual string DirectoryName => $"{App.Current.DataDirectory}\\Settings";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger for logging messages.</param>
        public SettingsStorageStrategy(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public Task<StorageLoadResult<string, TItem>> LoadAsync(string key)
        {
            var taskCompletionSource = new TaskCompletionSource<StorageLoadResult<string, TItem>>();
            var filePath = Environment.ExpandEnvironmentVariables($"{DirectoryName}\\{key}.json");

            if (!File.Exists(filePath))
            {
                var result = new StorageLoadResult<string, TItem>(key,
                    new StorageFailureDetail("The settings file could not be found.",
                        new FileNotFoundException("File does not exist or could not be found.", filePath)));

                _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                taskCompletionSource.SetResult(result);

                return taskCompletionSource.Task;
            }

            try
            {
                var jsonString = File.ReadAllText(filePath);
                var settingsObj = JsonConvert.DeserializeObject<TItem>(jsonString,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });

                taskCompletionSource.SetResult(new StorageLoadResult<string, TItem>(key, settingsObj));
            }
            catch (Exception ex)
            {
                var result = new StorageLoadResult<string, TItem>(key,
                    new StorageFailureDetail("Unable to load settings from file. See exception for more detail.", ex));

                _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                taskCompletionSource.SetResult(result);
            }

            return taskCompletionSource.Task;
        }

        public Task<StorageSaveResult<string, TItem>> SaveAsync(string key, TItem item)
        {
            var taskCompletionSource = new TaskCompletionSource<StorageSaveResult<string, TItem>>();
            var filePath = Environment.ExpandEnvironmentVariables($"{DirectoryName}\\{key}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    var result = new StorageSaveResult<string, TItem>(key, item,
                        new StorageFailureDetail("Unable to replace existing file.", ex));

                    _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                    taskCompletionSource.SetResult(result);

                    return taskCompletionSource.Task;
                }

                if (File.Exists(filePath))
                {
                    var result = new StorageSaveResult<string, TItem>(key, item,
                        new StorageFailureDetail("Unable to replace existing file."));

                    _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                    taskCompletionSource.SetResult(result);

                    return taskCompletionSource.Task;
                }
            }

            try
            {
                if (!Directory.Exists(Environment.ExpandEnvironmentVariables(DirectoryName)))
                    Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(DirectoryName));

                var jsonString = JsonConvert.SerializeObject(item, Formatting.Indented);
                File.WriteAllText(filePath, jsonString);

                if (File.Exists(filePath))
                {
                    taskCompletionSource.SetResult(new StorageSaveResult<string, TItem>(key, item));
                    return taskCompletionSource.Task;
                }
            }
            catch (Exception ex)
            {
                var result = new StorageSaveResult<string, TItem>(key, item,
                    new StorageFailureDetail("Unable to save to file. See the exception for more detail.", ex));

                _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                taskCompletionSource.SetResult(result);

                return taskCompletionSource.Task;
            }

            var result2 = new StorageSaveResult<string, TItem>(key, item,
                new StorageFailureDetail("Unable to save settings file."));

            _logger.Warn(result2.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result2.FailureDetail.Reason);
            taskCompletionSource.SetResult(result2);

            return taskCompletionSource.Task;
        }

        public Task<StorageDeleteResult<string>> DeleteAsync(string key, TItem item)
        {
            var taskCompletionSource = new TaskCompletionSource<StorageDeleteResult<string>>();
            var filePath = Environment.ExpandEnvironmentVariables($"{DirectoryName}\\{key}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    var result = new StorageDeleteResult<string>(key,
                        new StorageFailureDetail("Could not delete settings file. See exception for more detail.", ex));

                    _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                    taskCompletionSource.SetResult(result);

                    return taskCompletionSource.Task;
                }

                if (File.Exists(filePath))
                {
                    var result = new StorageDeleteResult<string>(key, new StorageFailureDetail("Unable to delete settings file."));

                    _logger.Warn(result.FailureDetail.Exception, "Settings: {0}\n\n{1}", key, result.FailureDetail.Reason);
                    taskCompletionSource.SetResult(result);

                    return taskCompletionSource.Task;
                }
            }

            taskCompletionSource.SetResult(new StorageDeleteResult<string>(key));

            return taskCompletionSource.Task;
        }

        #endregion
    }
}
