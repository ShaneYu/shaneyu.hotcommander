namespace ShaneYu.HotCommander.Settings.EventArgs
{
    /// <summary>
    /// Settings Event Args
    /// </summary>
    /// <typeparam name="T">Type of settings</typeparam>
    public class SettingsEventArgs<T> : System.EventArgs where T : class, new()
    {
        /// <summary>
        /// Gets the settings object
        /// </summary>
        public T Settings { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">The settings object</param>
        public SettingsEventArgs(T settings)
        {
            Settings = settings;
        }
    }
}
