using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MahApps.Metro;

namespace ShaneYu.HotCommander.UI.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for LoadingIndicator.xaml
    /// </summary>
    public partial class LoadingIndicator : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private string _progressText;
        private double _progressFontSize;
        private FontWeight _progressFontWeight;
        private Brush _progressForeground;
        private Brush _progressBackground;
        private double _progressOpacity;
        private Brush _progressSpinnerForeground;
        private double _progressSpinnerWidth;
        private double _progressSpinnerHeight;

        #endregion

        #region Properties

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                if (string.Compare(_progressText, value, StringComparison.Ordinal) != 0)
                {
                    _progressText = value;
                    RaisePropertyChanged();

                }
            }
        }

        public double ProgressFontSize
        {
            get { return _progressFontSize; }
            set
            {
                if (!Equals(_progressFontSize, value))
                {
                    _progressFontSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        public FontWeight ProgressFontWeight
        {
            get { return _progressFontWeight; }
            set
            {
                if (!Equals(_progressFontWeight, value))
                {
                    _progressFontWeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Brush ProgressForeground
        {
            get { return _progressForeground; }
            set
            {
                if (!Equals(_progressForeground, value))
                {
                    _progressForeground = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Brush ProgressBackground
        {
            get { return _progressBackground; }
            set
            {
                if (!Equals(_progressBackground, value))
                {
                    _progressBackground = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double ProgressOpacity
        {
            get { return _progressOpacity; }
            set
            {
                if (!Equals(_progressOpacity, value))
                {
                    _progressOpacity = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Brush ProgressSpinnerForeground
        {
            get { return _progressSpinnerForeground; }
            set
            {
                if (!Equals(_progressSpinnerForeground, value))
                {
                    _progressSpinnerForeground = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double ProgressSpinnerWidth
        {
            get { return _progressSpinnerWidth; }
            set
            {
                if (!Equals(_progressSpinnerWidth, value))
                {
                    _progressSpinnerWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double ProgressSpinnerHeight
        {
            get { return _progressSpinnerHeight; }
            set
            {
                if (!Equals(_progressSpinnerHeight, value))
                {
                    _progressSpinnerHeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is fired when a propertys' value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public LoadingIndicator()
        {
            _progressText = "Loading";
            _progressFontSize = 10;
            _progressFontWeight = FontWeights.SemiBold;
            _progressForeground = (SolidColorBrush)FindResource("AccentColorBrush");
            _progressBackground = Brushes.White;
            _progressOpacity = .85;
            _progressSpinnerForeground = (SolidColorBrush)FindResource("AccentColorBrush");
            _progressSpinnerWidth = 90;
            _progressSpinnerHeight = 90;

            try
            {
                var currentStyle = ThemeManager.DetectAppStyle(Application.Current);
                if (currentStyle.Item1.Name == "BaseDark")
                {
                    _progressBackground = Brushes.Black;
                }
            }
            catch { /* If detecting of theme fails, just use default of white. */ }

            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Private Methods

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the loading indicator.
        /// </summary>
        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide the loading indicator.
        /// </summary>
        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
