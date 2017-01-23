using System.ComponentModel;
using ShaneYu.HotCommander.Settings;

namespace ShaneYu.HotCommander.Modules
{
    public interface IModuleSettingsProvider<T> : ISettingsProvider<T> where T : class, INotifyPropertyChanged, new()
    {
        string ModuleName { get; }
    }
}