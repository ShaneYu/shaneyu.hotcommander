using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using Newtonsoft.Json;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Helpers;

namespace ShaneYu.HotCommander.Commands
{
    /// <summary>
    /// Validating Command Configuration Base
    /// </summary>
    public abstract class HotCommandConfigurationBase : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        private bool _isDirty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the configuration is dirty (one or more property values have changed).
        /// </summary>
        [JsonIgnore]
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("").
        /// </returns>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        [JsonIgnore]
        public virtual string this[string columnName]
            => ValidationHelper.GetValidationErrorMessageForProperty(this, columnName);

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        [JsonIgnore]
        public virtual string Error => string.Empty;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        protected HotCommandConfigurationBase()
        {
            PropertyChanged += (sender, args) => IsDirty = true;
        }

        #endregion

        #region Private Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
