using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

using MahApps.Metro.Controls;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Settings;

namespace ShaneYu.HotCommander.UI.WPF.Settings
{
    /// <summary>
    /// Application Settings
    /// </summary>
    [Serializable]
    public class ApplicationSettings : SettingsBase
    {
        #region Fields

        private Key _key = Key.K;
        private ModifierKeys _modifierKeys = ModifierKeys.Control | ModifierKeys.Alt;
        private string _accentColor = "Cobalt";
        private string _baseTheme = "Light";
        private int _maxVisibleResults = 6;
        private float _barWidth = 600;
        private bool _barWidthIsPercent;
        private float _barOffsetTop = 15;
        private bool _barOffsetTopIsPercent = true;
        private float _barOffsetBottom = 15;
        private bool _barOffsetBottomIsPercent = true;
        private bool _startWithWindows;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application accent color.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(0)]
        [DisplayName("Accent")]
        [Description("The desired application's accent color.")]
        [UIHint("AccentSelector")]
        public string AccentColor
        {
            get { return _accentColor; }
            set
            {
                if (string.Compare(_accentColor, value, StringComparison.Ordinal) != 0)
                {
                    _accentColor = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the application theme base color.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(1)]
        [DisplayName("Theme")]
        [Description("The desired application's base theme.")]
        [UIHint("ThemeSelector")]
        public string ThemeBase
        {
            get { return _baseTheme; }
            set
            {
                if (string.Compare(_baseTheme, value, StringComparison.Ordinal) != 0)
                {
                    _baseTheme = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of results to show in the command bar dropdown.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(2)]
        [DisplayName("Max Visible Results")]
        [Description("Determines the maximum number of results to show in the command bar list.")]
        public int MaxVisibleResults
        {
            get { return _maxVisibleResults; }
            set
            {
                if (!Equals(_maxVisibleResults, value))
                {
                    _maxVisibleResults = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the command bar.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(3)]
        [DisplayName("Bar Width")]
        [Description("Determines the width of the command bar.")]
        public float BarWidth
        {
            get { return _barWidth; }
            set
            {
                if (!Equals(_barWidth, value))
                {
                    _barWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the command bar width unit is percent (otherwise pixels).
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(4)]
        [DisplayName("Is Percentage")]
        [Description("Determines the whether the command bar width unit is percent or pixels.")]
        public bool BarWidthIsPercent
        {
            get { return _barWidthIsPercent; }
            set
            {
                if (!Equals(_barWidthIsPercent, value))
                {
                    _barWidthIsPercent = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the top offset of the command bar.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(5)]
        [DisplayName("Bar Offset Top")]
        [Description("Determines the top offset of the command bar.")]
        public float BarOffsetTop
        {
            get { return _barOffsetTop; }
            set
            {
                if (!Equals(_barOffsetTop, value))
                {
                    _barOffsetTop = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the command bar top offset unit is percent (otherwise pixels).
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(6)]
        [DisplayName("Is Percentage")]
        [Description("Determines the whether the command bar top offset unit is percent or pixels.")]
        public bool BarOffsetTopIsPercent
        {
            get { return _barOffsetTopIsPercent; }
            set
            {
                if (!Equals(_barOffsetTopIsPercent, value))
                {
                    _barOffsetTopIsPercent = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the bottom offset of the command bar.
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(7)]
        [DisplayName("Bar Offset Bottom")]
        [Description("Determines the bottom offset of the command bar.")]
        public float BarOffsetBottom
        {
            get { return _barOffsetBottom; }
            set
            {
                if (!Equals(_barOffsetBottom, value))
                {
                    _barOffsetBottom = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the command bar bottom offset unit is percent (otherwise pixels).
        /// </summary>
        [DisplayGroup("Appearance")]
        [DisplayOrder(8)]
        [DisplayName("Is Percentage")]
        [Description("Determines the whether the command bar bottom offset unit is percent or pixels.")]
        public bool BarOffsetBottomIsPercent
        {
            get { return _barOffsetBottomIsPercent; }
            set
            {
                if (!Equals(_barOffsetBottomIsPercent, value))
                {
                    _barOffsetBottomIsPercent = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the hot key 'Key'.
        /// </summary>
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Key Key
        {
            get { return _key; }
            set
            {
                if (!Equals(_key, value))
                {
                    _key = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(HotKey));
                }
            }
        }

        /// <summary>
        /// Gets or sets the hot key 'Modifier Keys'.
        /// </summary>
        [DisplayMode(DisplayMode.Hidden, DisplayMode.Hidden)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ModifierKeys ModifierKeys
        {
            get { return _modifierKeys; }
            set
            {
                if (!Equals(_modifierKeys, value))
                {
                    _modifierKeys = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(HotKey));
                }
            }
        }

        /// <summary>
        /// Gets or sets the hot key.
        /// </summary>
        [DisplayGroup("Behavior")]
        [DisplayOrder(0)]
        [Description("Determines the hot key to be pressed to activate the command bar.")]
        [JsonIgnore]
        public HotKey HotKey
        {
            get
            {
                return new HotKey(Key, ModifierKeys);
            }
            set
            {
                if (!Equals(HotKey, value))
                {
                    Key = value.Key;
                    ModifierKeys = value.ModifierKeys;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to startup with windows
        /// </summary>
        [DisplayGroup("Behavior")]
        [DisplayOrder(1)]
        [Description("Determines whether or not to launch the application on Windows startup.")]
        [JsonIgnore]
        public bool StartupWithWindows
        {
            get { return _startWithWindows; }
            set
            {
                if (!Equals(_startWithWindows, value))
                {
                    _startWithWindows = value;
                    RaisePropertyChanged();

                    if (_startWithWindows)
                        StartUpManager.AddApplicationToCurrentUserStartup();
                    else
                        StartUpManager.RemoveApplicationFromCurrentUserStartup();
                }
            }
        }

        #endregion

        #region Constructors

        public ApplicationSettings()
        {
            _startWithWindows = StartUpManager.IsApplicationRegisteredToStartupWithWindows();
        }

        #endregion
    }
}
