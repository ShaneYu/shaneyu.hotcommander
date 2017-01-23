using System.ComponentModel;
using ShaneYu.HotCommander.Settings;
using ShaneYu.HotCommander.Storage;

namespace ShaneYu.HotCommander.Modules
{
    public class ModuleSettingsProviderBase<T> : SettingsProviderBase<T> where T: class, INotifyPropertyChanged, new()
    {
        public ModuleSettingsProviderBase(IStorageStrategy<string, T> storageStrategy, string moduleName, string name, bool autoLoad = true)
            : base(storageStrategy, $"{moduleName}-{name}", autoLoad)
        {
        }
    }
}
