using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Helpers;
using ShaneYu.HotCommander.Logging;
using ShaneYu.HotCommander.UI.WPF.Models;
using ShaneYu.HotCommander.UI.WPF.Settings;
using ShaneYu.HotCommander.UI.WPF.UserControls;

namespace ShaneYu.HotCommander.UI.WPF.Windows
{
    /// <summary>
    /// Interaction logic for CommandCenter.xaml
    /// </summary>
    public partial class CommandCenter : MetroWindow
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IHotCommandManager _commandManager;
        private readonly CommandCenterViewModel _viewModel;
        private readonly ApplicationSettingsProvider _applicationSettingsProvider;

        private IHotCommand<IHotCommandConfiguration> _editingCommand;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger instance to use</param>
        /// <param name="commandManager">The command manager instance to use</param>
        /// <param name="viewModel">The view model instance to use</param>
        /// <param name="applicationSettingsProvider">The application settings provider instance to use</param>
        public CommandCenter(ILogger logger, IHotCommandManager commandManager, CommandCenterViewModel viewModel, ApplicationSettingsProvider applicationSettingsProvider)
        {
            _logger = logger;
            _commandManager = commandManager;
            _applicationSettingsProvider = applicationSettingsProvider;

            _viewModel = viewModel;
            _viewModel.FlyoutToggled += ViewModel_FlyoutToggled;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.IsBusy))
                {
                    if (_viewModel.IsBusy)
                        loadingIndicator.Show();
                    else
                        loadingIndicator.Hide();
                }
                
                CollectionViewSource.GetDefaultView(dataGridCommands.ItemsSource).Refresh();
            };

            DataContext = _viewModel;
            InitializeComponent();

            // If the editor flyout is closed, lets reject the changes.
            EditorFlyout.IsOpenChanged += (sender, args) =>
            {
                if (!EditorFlyout.IsOpen)
                {
                    modelEditor.RejectChanges();
                }
            };

            SettingsFlyout.IsOpenChanged += (sender, args) =>
            {
                if (!SettingsFlyout.IsOpen)
                {
                    var editor = SettingsFlyout.Content as ModelEditor;
                    editor?.RejectChanges();
                }
            };
        }

        #endregion

        #region Event Handlers

        private void ViewModel_FlyoutToggled(object sender, CommandCenterViewModel.Flyouts flyout)
        {
            AboutFlyout.IsOpen = (flyout == CommandCenterViewModel.Flyouts.About && !AboutFlyout.IsOpen);
            EditorFlyout.IsOpen = (flyout == CommandCenterViewModel.Flyouts.Editor && !EditorFlyout.IsOpen);
            SettingsFlyout.IsOpen = (flyout == CommandCenterViewModel.Flyouts.Settings && !SettingsFlyout.IsOpen);

            if (SettingsFlyout.IsOpen)
            {
                var editor = SettingsFlyout.Content as ModelEditor;

                if (editor == null)
                {
                    editor = new ModelEditor();
                    SettingsFlyout.Content = editor;
                }

                editor.SetData(_applicationSettingsProvider.Settings);

                editor.Rejected += (o, args) =>
                {
                    SettingsFlyout.IsOpen = false;
                    SettingsFlyout.Content = null;
                };

                editor.Accepting += async (o, args) =>
                {
                    if (args.Reject) return;

                    var result = (await _applicationSettingsProvider.SaveAsync()).Success;

                    if (result)
                    {
                        SettingsFlyout.IsOpen = false;
                        await this.ShowMessageAsync("Updated", "The settings were updated successfully.");
                    }
                    else
                    {
                        args.Reject = true;
                        SettingsFlyout.IsOpen = false;
                        await this.ShowMessageAsync("Failure", "The settings could not be updated.");
                    }
                };
            }
        }

        private void DataGridRowDoubleClick_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            EditCommand((row?.DataContext as CommandCenterViewModel.CommandData)?.Command);
        }

        private void EditCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            EditCommand((button?.DataContext as CommandCenterViewModel.CommandData)?.Command);
        }

        private async void DeleteCommandButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var command = (button?.DataContext as CommandCenterViewModel.CommandData)?.Command;

            if (command != null)
            {
                var settings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No - Don't Delete"
                };

                var result =
                    await
                        this.ShowMessageAsync("Delete Command",
                            $"Are you sure that you want to delete the following command?\n\nId: {command.Configuration.Id}\nName: {command.Configuration.Name}\nDescription: {command.Configuration.Description}\nType: {command.GetType().Name}",
                            MessageDialogStyle.AffirmativeAndNegative, settings);

                if (result == MessageDialogResult.Affirmative)
                {
                    _viewModel.IsBusy = true;

                    if (await _commandManager.DeleteAsync(command.Configuration.Id))
                    {
                        _viewModel.IsBusy = false;
                        await this.ShowMessageAsync("Deleted", "The command has been successfully deleted.");
                    }
                    else
                    {
                        _viewModel.IsBusy = false;
                        await this.ShowMessageAsync("Not Deleted", "Unsuccessful, the command could not be deleted.");
                    }
                }
            }
        }

        private void CreateNewButtonClick_OnHandler(object sender, RoutedEventArgs e)
        {
            dynamic dataContext = ((Control)e.Source).DataContext;

            var commandType = (Type)dataContext.CommandType;
            var configType = commandType.BaseType.GenericTypeArguments[0];

            var config = Activator.CreateInstance(configType);
            _editingCommand = (IHotCommand<IHotCommandConfiguration>)Activator.CreateInstance(commandType, config);

            modelEditor.SetData((INotifyPropertyChanged)_editingCommand.Configuration);
            modelEditor.Rejected += (o, args) => EditorFlyout.IsOpen = false;
            modelEditor.Accepting += async (o, args) =>
            {
                if (args.Reject) return;

                _viewModel.IsBusy = true;
                var result = await _commandManager.CreateAsync(_editingCommand);

                if (result)
                {
                    _viewModel.IsBusy = false;
                    EditorFlyout.IsOpen = false;
                    _editingCommand = null;
                    await this.ShowMessageAsync("Created", "The new command was created successfully.");
                }
                else
                {
                    args.Reject = true;
                    _viewModel.IsBusy = false;
                    await this.ShowMessageAsync("Failure", "The new command could not be created.");
                }
            };

            EditorFlyout.IsOpen = true;
        }

        #endregion

        #region Private Methods

        private void EditCommand(IHotCommand<IHotCommandConfiguration> command)
        {
            _editingCommand = command;

            modelEditor.SetData((INotifyPropertyChanged)_editingCommand.Configuration);

            modelEditor.Accepting += async (o, args) =>
            {
                if (args.Reject) return;

                _viewModel.IsBusy = true;
                var cmd = _commandManager.Get(_editingCommand.Configuration.Id);
                PropertyCopier.Copy(_editingCommand.Configuration, cmd.Configuration);
                var result = await _commandManager.SaveAsync(cmd.Configuration.Id);

                if (result)
                {
                    _viewModel.IsBusy = false;
                    EditorFlyout.IsOpen = false;
                    _editingCommand = null;
                    await this.ShowMessageAsync("Updated", "The command was successfully updated.");
                }
                else
                {
                    args.Reject = true;
                    _viewModel.IsBusy = false;
                    await this.ShowMessageAsync("Failure", "The command could not be updated.");
                }
            };

            modelEditor.Rejected += (sender, args) => EditorFlyout.IsOpen = false;

            EditorFlyout.IsOpen = true;
        }

        #endregion
    }
}
