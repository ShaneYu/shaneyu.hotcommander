using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Logging;
using ShaneYu.HotCommander.UI.WPF.Converters;
using ShaneYu.HotCommander.UI.WPF.Helpers;

namespace ShaneYu.HotCommander.UI.WPF.Models
{
    /// <summary>
    /// Command Center View Model
    /// </summary>
    public class CommandCenterViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IHotCommandManager _commandManager;
        private readonly ObservableCollection<CommandData> _commands;
        private readonly CollectionViewSource _commandsViewSource;
        private readonly dynamic _commandTypes;

        private bool _isBusy;

        private bool _groupingByModule;
        private bool _groupingByType;
        private bool _groupingByStatus;

        private string _filterSearch;
        private bool? _filterStatus;
        private Type _filterType;

        #endregion

        #region Properties

        public ObservableCollection<CommandData> Commands => _commands;

        public ICollectionView CommandsView => _commandsViewSource.View;

        public CollectionViewSource CommandsViewSource => _commandsViewSource;

        public dynamic CommandTypes => _commandTypes;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool GroupingByModule
        {
            get { return _groupingByModule; }
            set
            {
                if (_groupingByModule != value)
                {
                    _groupingByModule = value;
                    RaisePropertyChanged();

                    UpdateGroupings();
                }
            }
        }

        public bool GroupingByType
        {
            get { return _groupingByType; }
            set
            {
                if (_groupingByType != value)
                {
                    _groupingByType = value;
                    RaisePropertyChanged();

                    UpdateGroupings();
                }
            }
        }

        public bool GroupingByStatus
        {
            get { return _groupingByStatus; }
            set
            {
                if (_groupingByStatus != value)
                {
                    _groupingByStatus = value;
                    RaisePropertyChanged();

                    UpdateGroupings();
                }
            }
        }

        public string FilterSearch
        {
            get { return _filterSearch; }
            set
            {
                if (string.Compare(_filterSearch, value, StringComparison.InvariantCulture) != 0)
                {
                    _filterSearch = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Type FilterType
        {
            get { return _filterType; }
            set
            {
                if (_filterType != value)
                {
                    _filterType = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool? FilterStatus
        {
            get { return _filterStatus; }
            set
            {
                if (_filterStatus != value)
                {
                    _filterStatus = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public Version ApplicationVersion => App.Current.Version;

        /// <summary>
        /// Gets the toggle about flyout command.
        /// </summary>
        public ICommand ToggleAboutFlyout { get; }

        /// <summary>
        /// Gets the toggle settings flyout command.
        /// </summary>
        public ICommand ToggleSettingsFlyout { get; }

        /// <summary>
        /// Gets the toggle editor flyout command.
        /// </summary>
        public ICommand ToggleEditorFlyout { get; }

        #endregion

        #region Events

        /// <summary>
        /// Event that is fired when a flyout is toggled.
        /// </summary>
        public event EventHandler<Flyouts> FlyoutToggled;

        /// <summary>
        /// Event that is fired when a property's value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger instance to use</param>
        /// <param name="commandManager">The command manager instance to use</param>
        public CommandCenterViewModel(ILogger logger, IHotCommandManager commandManager)
        {
            _logger = logger;
            _commandManager = commandManager;

            _commands = new ObservableCollection<CommandData>();
            _commandsViewSource = new CollectionViewSource { Source = _commands };

            _commandsViewSource.Filter += (sender, args) =>
            {
                args.Accepted = CommandsViewFilter(args.Item);
            };
            //CommandsView.Filter = CommandsViewFilter;

            _commandsViewSource.SortDescriptions.Add(new SortDescription("ModuleName", ListSortDirection.Ascending));
            _commandsViewSource.SortDescriptions.Add(new SortDescription("CommandType.Name", ListSortDirection.Ascending));
            _commandsViewSource.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
            _commandsViewSource.SortDescriptions.Add(new SortDescription("Command.Configuration.Name", ListSortDirection.Ascending));

            var liveView = _commandsViewSource.View as ICollectionViewLiveShaping;
            if (liveView != null)
            {
                liveView.LiveFilteringProperties.Add("ModuleName");
                liveView.LiveFilteringProperties.Add("CommandType.Name");
                liveView.LiveFilteringProperties.Add("Status");
                liveView.LiveFilteringProperties.Add("Command.Configuration.Name");

                liveView.IsLiveFiltering = true;
                liveView.IsLiveSorting = true;
            }

            _commandTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsInterface && typeof(IHotCommand<IHotCommandConfiguration>).IsAssignableFrom(t) && t.GetCustomAttribute<InternalHotCommandAttribute>() == null)
                    .Select(t => new { Name = StringHelper.SpaceOutPascal(t.Name.Replace("Command", "")), CommandType = t })
                    .ToArray();

            _commandManager.CommandSaved += (sender, command) => AddOrUpdateCommand(command);
            _commandManager.CommandLoaded += (sender, command) => UpdateCommands();
            _commandManager.CommandDeleted += (sender, guid) => RemoveCommand(guid);
            _commandManager.CommandRegistered += (sender, command) => AddOrUpdateCommand(command);
            _commandManager.CommandCreated += (sender, command) => AddOrUpdateCommand(command);

            ToggleAboutFlyout = new RelayCommand(obj => FlyoutToggled?.Invoke(this, Flyouts.About), obj => true);
            ToggleSettingsFlyout = new RelayCommand(obj => FlyoutToggled?.Invoke(this, Flyouts.Settings), obj => true);
            ToggleEditorFlyout = new RelayCommand(obj => FlyoutToggled?.Invoke(this, Flyouts.Editor), obj => true);

            UpdateCommands();
        }

        private void RemoveCommand(Guid id)
        {
            var existingCommand = _commands.FirstOrDefault(cd => cd.Command.Configuration.Id == id);

            if (existingCommand != null)
            {
                _commands.Remove(existingCommand);
            }
        }

        private void AddOrUpdateCommand(IHotCommand<IHotCommandConfiguration> command)
        {
            RemoveCommand(command.Configuration.Id);
            _commands.Add(new CommandData { Command = command });
        }

        #endregion

        #region Private Methods

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CommandsViewFilter(object o)
        {
            var commandData = o as CommandData;

            if (commandData?.Command == null) return false;

            if (FilterStatus.HasValue && commandData.Command.Configuration.IsEnabled != FilterStatus.Value)
            {
                return false;
            }

            if (FilterType != null && commandData.CommandType != FilterType)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(FilterSearch) &&
                !Regex.IsMatch(commandData.Command.Configuration.Name, FilterSearch, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(commandData.Command.Configuration.Description, FilterSearch, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
            {
                return false;
            }

            return true;
        }

        private void UpdateGroupings()
        {
            if (CommandsView.CanGroup)
            {
                var properties = new List<string>();
                CommandsView.GroupDescriptions.Clear();

                if (GroupingByModule)
                {
                    CommandsView.GroupDescriptions.Add(new PropertyGroupDescription("ModuleName"));
                    properties.Add("ModuleName");
                }

                if (GroupingByType)
                {
                    CommandsView.GroupDescriptions.Add(new PropertyGroupDescription("CommandType", new TypeToNameConverter()));
                    properties.Add("CommandType");
                }

                if (GroupingByStatus)
                {
                    CommandsView.GroupDescriptions.Add(new PropertyGroupDescription("Status"));
                    properties.Add("Status");
                }

                //CommandsView.ActivateLiveGrouping(properties.ToArray());
                CommandsView.Refresh();
            }
        }

        #endregion

        #region Public Methods

        public void UpdateCommands()
        {
            IsBusy = true;
            _commands.Clear();

            foreach (var command in _commandManager.GetAll(true, true).ToArray())
            {
                _commands.Add(new CommandData { Command = command });
            }

            IsBusy = false;
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// The available flyouts for toggling.
        /// </summary>
        public enum Flyouts
        {
            /// <summary>
            ///  About Flyout
            /// </summary>
            About,

            /// <summary>
            /// Settings Flyout
            /// </summary>
            Settings,

            /// <summary>
            /// Editor Flyout
            /// </summary>
            Editor
        }

        public class CommandData
        {
            public IHotCommand<IHotCommandConfiguration> Command { get; set; }

            public Type CommandType => Command.GetType();

            public string ModuleName => StringHelper.SpaceOutPascal(Command.GetType()
                            .Assembly.GetName()
                            .Name.Replace("ShaneYu.HotCommander", "Core")
                            .Replace("HotCommander", "UI")
                            .Replace(".", " "));

            public string Status => Command.Configuration.IsEnabled ? "Enabled" : "Disabled";
        }

        #endregion
    }
}
