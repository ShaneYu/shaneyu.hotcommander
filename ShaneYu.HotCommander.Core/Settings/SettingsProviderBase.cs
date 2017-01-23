using System;
using System.ComponentModel;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Helpers;
using ShaneYu.HotCommander.Settings.EventArgs;
using ShaneYu.HotCommander.Storage;

namespace ShaneYu.HotCommander.Settings
{
    /// <summary>
    /// Settings Provider Base
    /// </summary>
    /// <typeparam name="T">The type of settings being provided</typeparam>
    public class SettingsProviderBase<T> : ISettingsProvider<T> where T : class, INotifyPropertyChanged, new()
    {
        #region Events

        /// <summary>
        /// Event that is fired when the settings have been loaded.
        /// </summary>
        public event EventHandler<SettingsEventArgs<T>> Loaded;

        /// <summary>
        /// Event that is fired when the settings file to load.
        /// </summary>
        public event EventHandler<SettingsFailureEventArgs> LoadFailed;

        /// <summary>
        /// Event that is fired when the settings have been saved.
        /// </summary>
        public event EventHandler<SettingsEventArgs<T>> Saved;

        /// <summary>
        /// Event that is fired when the settings fail to save.
        /// </summary>
        public event EventHandler<SettingsFailureEventArgs> SaveFailed;

        /// <summary>
        /// Event that is fired when the settings have been deleted.
        /// </summary>
        public event EventHandler<SettingsEventArgs<T>> Deleted;

        /// <summary>
        /// Event that is fired when the settings fail to be deleted.
        /// </summary>
        public event EventHandler<SettingsFailureEventArgs> DeleteFailed;

        #endregion

        #region Fields

        private readonly T _settings;
        private readonly string _name;
        private readonly IStorageStrategy<string, T> _storageStrategy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the settings being provided.
        /// </summary>
        public T Settings => _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageStrategy">The storage strategy to use</param>
        /// <param name="name">The settings unquie name</param>
        public SettingsProviderBase(IStorageStrategy<string, T> storageStrategy, string name, bool autoLoad = true)
        {
            _name = name;
            _settings = new T();
            _storageStrategy = storageStrategy;

            if (autoLoad)
                LoadAsync().Wait();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public async Task<SettingsResult> LoadAsync()
        {
            var result = await _storageStrategy.LoadAsync(_name);

            if (result.Success)
            {
                PropertyCopier.Copy(result.Item, Settings);
                Loaded?.Invoke(this, new SettingsEventArgs<T>(Settings));
                return new SettingsResult();
            }

            LoadFailed?.Invoke(this, new SettingsFailureEventArgs(result.FailureDetail.Reason, result.FailureDetail.Exception));
            return new SettingsResult(result.FailureDetail);
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public async Task<SettingsResult> SaveAsync()
        {
            var result = await _storageStrategy.SaveAsync(_name, Settings);

            if (result.Success)
            {
                Saved?.Invoke(this, new SettingsEventArgs<T>(Settings));
                return new SettingsResult();
            }

            SaveFailed?.Invoke(this, new SettingsFailureEventArgs(result.FailureDetail.Reason, result.FailureDetail.Exception));
            return new SettingsResult(result.FailureDetail);
        }

        /// <summary>
        /// Deletes the settings.
        /// </summary>
        /// <returns>The delete result.</returns>
        public async Task<SettingsResult> DeleteAsync()
        {
            var result = await _storageStrategy.DeleteAsync(_name, Settings);

            if (result.Success)
            {
                Deleted?.Invoke(this, new SettingsEventArgs<T>(Settings));
                return new SettingsResult();
            }

            DeleteFailed?.Invoke(this, new SettingsFailureEventArgs(result.FailureDetail.Reason, result.FailureDetail.Exception));
            return new SettingsResult(result.FailureDetail);
        }

        #endregion
    }
}
