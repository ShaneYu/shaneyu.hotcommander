using System;
using System.ComponentModel;
using System.Threading.Tasks;

using ShaneYu.HotCommander.Settings.EventArgs;

namespace ShaneYu.HotCommander.Settings
{
    public interface ISettingsProvider<T> where T : class, INotifyPropertyChanged, new()
    {
        /// <summary>
        /// Event that is fired when the settings have been loaded.
        /// </summary>
        event EventHandler<SettingsEventArgs<T>> Loaded;

        /// <summary>
        /// Event that is fired when the settings file to load.
        /// </summary>
        event EventHandler<SettingsFailureEventArgs> LoadFailed;

        /// <summary>
        /// Event that is fired when the settings have been saved.
        /// </summary>
        event EventHandler<SettingsEventArgs<T>> Saved;

        /// <summary>
        /// Event that is fired when the settings fail to save.
        /// </summary>
        event EventHandler<SettingsFailureEventArgs> SaveFailed;

        /// <summary>
        /// Event that is fired when the settings have been deleted.
        /// </summary>
        event EventHandler<SettingsEventArgs<T>> Deleted;

        /// <summary>
        /// Event that is fired when the settings fail to be deleted.
        /// </summary>
        event EventHandler<SettingsFailureEventArgs> DeleteFailed;

        /// <summary>
        /// Gets the settings being provided.
        /// </summary>
        T Settings { get; }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>The load result.</returns>
        Task<SettingsResult> LoadAsync();

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <returns>The save result.</returns>
        Task<SettingsResult> SaveAsync();

        /// <summary>
        /// Deletes the settings.
        /// </summary>
        /// <returns>The delete result.</returns>
        Task<SettingsResult> DeleteAsync();
    }
}
