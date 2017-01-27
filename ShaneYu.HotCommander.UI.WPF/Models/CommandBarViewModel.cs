using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Logging;
using ShaneYu.HotCommander.Searching;
using ShaneYu.HotCommander.UI.WPF.Searching;
using ShaneYu.HotCommander.UI.WPF.Extensions;

namespace ShaneYu.HotCommander.UI.WPF.Models
{
    /// <summary>
    /// Command Bar View Model
    /// </summary>
    public class CommandBarViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IHotCommandManager _commandManager;
        private readonly ISearchStrategy<TextBlock> _searchStrategy;

        private string _searchTerm;
        private TextBlock[] _searchResults = new TextBlock[0];
        private TextBlock _selectedResult;

        private IHotCommand<IHotCommandConfiguration> _lockedCommand;
        private IHotCommandStep _currentStep;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                if (string.Compare(_searchTerm, value, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    _searchTerm = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        /// <summary>
        /// Gets or sets the locked parts.
        /// </summary>
        public ObservableCollection<string> LockedParts { get; }

        /// <summary>
        /// Gets the search results.
        /// </summary>
        public TextBlock[] SearchResults => _searchResults;

        public Visibility SearchResultsVisibility => _searchResults.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Gets or sets the selected search result.
        /// </summary>
        public TextBlock SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if (!Equals(_selectedResult, value))
                {
                    _selectedResult = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger instance to use</param>
        /// <param name="commandManager">The command manager instance to use</param>
        public CommandBarViewModel(ILogger logger, IHotCommandManager commandManager)
        {
            _logger = logger;
            _commandManager = commandManager;
            _searchStrategy = new CustomSearchStrategy();
            LockedParts = new ObservableCollection<string>();
        }

        #endregion

        #region Private Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PerformSearch()
        {
            var results = new List<TextBlock>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                if (_lockedCommand == null)
                {
                    var commands = _commandManager.Search(_searchStrategy, SearchTerm);
                    results.AddRange(commands);
                }
                else if (_currentStep != null && !_currentStep.IsSet && _currentStep.Options != null)
                {
                    // TODO: Perhaps command step options should also partake in a search strategy and that both cmd and step matches can show description (user preference).
                    var matches =
                        _currentStep.Options.Where(x => x.ToLower().StartsWith(SearchTerm.ToLower()))
                            .OrderBy(x => x)
                            .Select(x => new TextBlock {Text = x});

                    results.AddRange(matches);
                }
            }

            _searchResults = results.ToArray();
            OnPropertyChanged(nameof(SearchResults));
            OnPropertyChanged(nameof(SearchResultsVisibility));
            SelectedResult = SearchResults.FirstOrDefault();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Undos all steps and locked command in a multi-step scenario.
        /// </summary>
        public void UndoAll()
        {
            _lockedCommand?.NextStep?.Reset();
            _lockedCommand = null;
            _currentStep = null;

            SearchTerm = string.Empty;
            PerformSearch();
            LockedParts.Clear();
        }

        /// <summary>
        /// Undos the previous step or locked command in a multi-step scenario.
        /// </summary>
        public void Undo()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                return;
            }

            if (_currentStep != null)
            {
                _currentStep = _currentStep.PreviousStep;

                if (_currentStep == null)
                {
                    _lockedCommand = null;
                    LockedParts.Clear();
                }
                else
                {
                    _currentStep.Reset();

                    if (LockedParts.Count > 0)
                    {
                        LockedParts.RemoveAt(LockedParts.Count - 1);
                    }
                }
            }
            else
            {
                // If current step is null, then a command should not have been locked in yet.
                // But just in case, lets clear the locked in command to be sure.
                _lockedCommand = null;
                LockedParts.Clear();
            }
        }

        /// <summary>
        /// Select the first result.
        /// </summary>
        public void First()
        {
            SelectedResult = SearchResults.FirstOrDefault();
        }

        /// <summary>
        /// Select the result <paramref name="step"/> before current.
        /// </summary>
        /// <param name="step">How many results previous to select.</param>
        public void Previous(int step = 1)
        {
            var index = SearchResults.IndexOf(SelectedResult) - step;
            index = Math.Max(0, index);

            SelectedResult = SearchResults.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Select the result <paramref name="step"/> after current.
        /// </summary>
        /// <param name="step">How many results next to select.</param>
        public void Next(int step = 1)
        {
            var index = SearchResults.IndexOf(SelectedResult) + step;
            index = Math.Min(SearchResults.Length - 1, index);

            SelectedResult = SearchResults.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Select the last result.
        /// </summary>
        public void Last()
        {
            SelectedResult = SearchResults.LastOrDefault();
        }

        public void NextPartOrExecute()
        {
            Execute(false);
        }

        /// <summary>
        /// Execute command of selected result.
        /// </summary>
        public void Execute(bool executeDefaults = true)
        {
            if (_lockedCommand == null && SelectedResult?.Tag != null)
            {
                var cmdId = (Guid) SelectedResult.Tag;
                _lockedCommand = _commandManager.Get(cmdId);

                if (_lockedCommand != null)
                {
                    if (_lockedCommand.NextStep == null || (!_lockedCommand.NextStep.IsRequired && executeDefaults))
                    {
                        _lockedCommand.Execute();
                        _lockedCommand?.NextStep?.Reset();
                        _lockedCommand = null;
                        LockedParts.Clear();
                        SearchTerm = string.Empty;
                        PerformSearch();
                    }
                    else
                    {
                        _currentStep = _lockedCommand.NextStep;
                        LockedParts.Add(_lockedCommand.Configuration.Name);
                        SearchTerm = string.Empty;
                        PerformSearch();
                    }
                }
            }
            else if (_lockedCommand != null && _currentStep != null)
            {
                if (_currentStep.Options != null && SelectedResult != null)
                {
                    _currentStep.SetData(SelectedResult.Text);
                }
                else if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    _currentStep.SetData(SearchTerm);
                }

                if (_currentStep.IsRequired && !_currentStep.IsSet)
                {
                    // Prevent current step locking in if it hasn't actually been set.
                    // Each command step can validate the data being set against what it allows, if it 
                    // doesn't accept the data, it will not persist it and IsSet will be false.
                    return;
                }

                if (_currentStep.NextStep == null || (!_currentStep.NextStep.IsRequired && executeDefaults))
                {
                    _lockedCommand.Execute();
                    _lockedCommand?.NextStep.Reset();
                    _lockedCommand = null;
                    _currentStep = null;
                    LockedParts.Clear();
                    SearchTerm = string.Empty;
                    PerformSearch();
                }
                else
                {
                    LockedParts.Add(_currentStep.Data);
                    _currentStep = _currentStep.NextStep;
                    SearchTerm = string.Empty;
                    PerformSearch();
                }
            }
        }

        #endregion
    }
}
