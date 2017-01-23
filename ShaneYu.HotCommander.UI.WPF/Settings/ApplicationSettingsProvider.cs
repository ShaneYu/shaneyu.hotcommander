using ShaneYu.HotCommander.Settings;
using ShaneYu.HotCommander.Storage;

namespace ShaneYu.HotCommander.UI.WPF.Settings
{
    /// <summary>
    /// Application Settings Provider
    /// </summary>
    public class ApplicationSettingsProvider : SettingsProviderBase<ApplicationSettings>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageStrategy">The storage strategy to use</param>
        public ApplicationSettingsProvider(IStorageStrategy<string, ApplicationSettings> storageStrategy)
            : base(storageStrategy, App.Current.ApplicationSettingsName)
        {
        }

        #endregion
    }
}
