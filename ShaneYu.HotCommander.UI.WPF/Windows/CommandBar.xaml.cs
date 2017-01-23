using System;
using System.Windows;
using System.Windows.Input;

using MahApps.Metro.Controls;

using ShaneYu.HotCommander.UI.WPF.Extensions;
using ShaneYu.HotCommander.UI.WPF.Helpers;
using ShaneYu.HotCommander.UI.WPF.Models;
using ShaneYu.HotCommander.UI.WPF.Settings;

namespace ShaneYu.HotCommander.UI.WPF.Windows
{
    /// <summary>
    /// Interaction logic for CommandBar.xaml
    /// </summary>
    public partial class CommandBar : MetroWindow
    {
        #region Fields

        private readonly CommandBarViewModel _viewModel;
        private readonly ApplicationSettingsProvider _applicationSettingsProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewModel">The view model instance to use</param>
        /// <param name="applicationSettingsProvider">The <see cref="ApplicationSettingsProvider"/> to use</param>
        public CommandBar(CommandBarViewModel viewModel, ApplicationSettingsProvider applicationSettingsProvider)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            _applicationSettingsProvider = applicationSettingsProvider;

            InitializeComponent();

            if (!_applicationSettingsProvider.Settings.BarWidthIsPercent)
            {
                // If width of bar is in pixels, set it now...no need to set it dynamically each time the bar is shown.
                Width = MinWidth = MaxWidth = _applicationSettingsProvider.Settings.BarWidth;
            }
        }

        #endregion

        #region Event Handlers

        private void CommandBar_OnDeactivated(object sender, EventArgs e)
        {
            _viewModel.UndoAll();
            Hide();
        }

        private void CommandBar_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _viewModel.UndoAll();
                    Hide();
                    break;

                case Key.Back:
                    _viewModel.Undo();
                    break;

                case Key.Home:
                    _viewModel.First();
                    break;

                case Key.Up:
                    _viewModel.Previous();
                    break;

                case Key.PageUp:
                    _viewModel.Previous(5);
                    break;

                case Key.PageDown:
                    _viewModel.Next(5);
                    break;

                case Key.Down:
                    _viewModel.Next();
                    break;

                case Key.End:
                    _viewModel.Last();
                    break;

                case Key.Enter:
                    _viewModel.Execute();
                    if (_viewModel.LockedParts.Count == 0)
                    {
                        // As there is multi-step commands, only hide the command bar if execution happened and locked parts is cleared.
                        Hide();
                        _viewModel.UndoAll();
                    }
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void CalculateWindowPosition(Window window, Win32API.W32MonitorInfo monitorInfo)
        {
            var monitorRect = monitorInfo.WorkArea;
            var monitorWidth = monitorRect.Right - monitorRect.Left;
            var monitorHeight = monitorRect.Bottom - monitorRect.Top;

            if (_applicationSettingsProvider.Settings.BarWidthIsPercent)
            {
                // This will handle setting command bar width to X percentage of active monitor.
                var percentage = _applicationSettingsProvider.Settings.BarWidth/100;
                window.Width = MinWidth = MaxWidth = monitorWidth*percentage;
            }
            else
            {
                window.Width = MinWidth = MaxWidth = _applicationSettingsProvider.Settings.BarWidth;
            }

            var windowWidth50Percent = Math.Round(window.ActualWidth / 2);
            var monitorWidth50Percent = monitorWidth * .5;
            var desiredLeft = monitorRect.Left + (monitorWidth50Percent - windowWidth50Percent);

            window.Left = desiredLeft;

            if (_applicationSettingsProvider.Settings.BarOffsetTopIsPercent)
            {
                var percentage = _applicationSettingsProvider.Settings.BarOffsetTop/100;
                var monitorHeight15Percent = monitorHeight*percentage;
                var desiredTop = monitorRect.Top + monitorHeight15Percent;

                window.Top = desiredTop;
            }
            else
            {
                window.Top = _applicationSettingsProvider.Settings.BarOffsetTop;
            }

            // Make sure the window is not set to appear offscreen or too close to the bottom that results cannot be seen.
            if (window.Top > monitorHeight - (window.ActualHeight * 3))
            {
                window.Top = monitorHeight - (window.ActualHeight * 3);
            }

            if (_applicationSettingsProvider.Settings.BarOffsetBottomIsPercent)
            {
                var percentage = _applicationSettingsProvider.Settings.BarOffsetBottom / 100;
                var monitorHeightPercentage = monitorHeight * percentage;
                var desiredMaxHeight = monitorHeight - monitorHeightPercentage;
                desiredMaxHeight = desiredMaxHeight - (float) window.Top;

                if (desiredMaxHeight < window.ActualHeight * 3)
                {
                    desiredMaxHeight = (float) window.ActualHeight * 3;
                }

                window.MaxHeight = desiredMaxHeight;
            }
            else
            {
                var desiredMaxHeight = monitorHeight - _applicationSettingsProvider.Settings.BarOffsetBottom;
                desiredMaxHeight = desiredMaxHeight -(float) window.Top;

                if (desiredMaxHeight < window.ActualHeight * 3)
                {
                    desiredMaxHeight = (float) window.ActualHeight * 3;
                }

                window.MaxHeight = desiredMaxHeight;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the command bar and focus in textbox.
        /// </summary>
        public void ShowAndFocus()
        {
            this.ShowOnActiveMonitor(CalculateWindowPosition);
            Activate();
            Focus();

            _viewModel.First();

            txtSearchTerm.SelectAll();
            txtSearchTerm.Focus();
        }

        #endregion
    }
}
