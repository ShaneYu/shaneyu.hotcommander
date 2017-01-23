using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using Autofac;

using MahApps.Metro;
using MahApps.Metro.Controls;

using Newtonsoft.Json;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Helpers;
using ShaneYu.HotCommander.IoC;
using ShaneYu.HotCommander.UI.WPF.Helpers;
using ShaneYu.HotCommander.UI.WPF.Models;

using Binding = System.Windows.Data.Binding;
using CharacterCasing = System.Windows.Controls.CharacterCasing;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using NumericUpDown = MahApps.Metro.Controls.NumericUpDown;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace ShaneYu.HotCommander.UI.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ModelEditor.xaml
    /// </summary>
    public partial class ModelEditor : UserControl, IRevertibleChangeTracking, INotifyPropertyChanged
    {
        #region Fields

        private bool _uiBuilt;
        private bool _isChanged;
        private INotifyPropertyChanged _data;
        private INotifyPropertyChanged _dataClone;
        private bool _editMode;

        private string _acceptButtonText = "Save";
        private string _rejectButtonText = "Cancel";

        private static readonly List<OpenFileDialog> OpenFileDialogs = new List<OpenFileDialog>();
        private static readonly List<FolderBrowserDialog> OpenFolderDialogs = new List<FolderBrowserDialog>();

        #endregion

        #region Events

        /// <summary>
        /// Event that is fired when a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event that is fired when the changes are being accepted.
        /// </summary>
        public event EventHandler<ModelEditorAcceptingEventArgs> Accepting;

        /// <summary>
        /// Event that is fired when the changes have been accepted.
        /// </summary>
        public event EventHandler Accepted;

        /// <summary>
        /// Event that is fired when the changes have been rejected.
        /// </summary>
        public event EventHandler Rejected;

        #endregion

        #region Properties

        public INotifyPropertyChanged DataModel => _data;

        /// <summary>
        /// Gets whether any of the data has changed.
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if (_isChanged != value)
                {
                    _isChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the accept button text.
        /// </summary>
        public string AcceptButtonText
        {
            get { return _acceptButtonText; }
            set
            {
                if (string.Compare(_acceptButtonText, value, StringComparison.Ordinal) != 0)
                {
                    _acceptButtonText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the reject button text.
        /// </summary>
        public string RejectButtonText
        {
            get { return _rejectButtonText; }
            set
            {
                if (string.Compare(_rejectButtonText, value, StringComparison.Ordinal) != 0)
                {
                    _rejectButtonText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command for accepting the changes.
        /// </summary>
        public ICommand AcceptChangesCommand { get; }

        /// <summary>
        /// Gets the command for rejecting the changes.
        /// </summary>
        public ICommand RejectChangesCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelEditor()
        {
            DataContext = this;
            AcceptChangesCommand = new RelayCommand(AcceptChanges, x => IsChanged);
            RejectChangesCommand = new RelayCommand(RejectChanges);

            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_uiBuilt)
                IsChanged = true;
        }

        #endregion

        #region Private Methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static INotifyPropertyChanged CloneData(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            return (INotifyPropertyChanged) JsonConvert.DeserializeObject(jsonString, data.GetType());
        }

        private void RemoveEventHandlers()
        {
            if (Accepting != null)
            {
                foreach (var handler in Accepting.GetInvocationList())
                {
                    Accepting -= (EventHandler<ModelEditorAcceptingEventArgs>) handler;
                }
            }

            if (Accepted != null)
            {
                foreach (var handler in Accepted.GetInvocationList())
                {
                    Accepted -= (EventHandler)handler;
                }
            }

            if (Rejected != null)
            {
                foreach (var handler in Rejected.GetInvocationList())
                {
                    Rejected -= (EventHandler)handler;
                }
            }
        }

        private void RemoveDataPropertyChangedHandler()
        {
            if (_data != null)
            {
                _data.PropertyChanged -= Model_OnPropertyChanged;
            }
        }

        private void AttachDataPropertyChangedHandler()
        {
            if (_data != null)
            {
                _data.PropertyChanged += Model_OnPropertyChanged;
            }
        }

        private void BuildDynamicUI()
        {
            spGroups.Children.Clear();
            OpenFileDialogs.Clear();
            OpenFolderDialogs.Clear();

            var dataType = _data.GetType();
            var groupOrders = dataType.GetCustomAttributes<DisplayGroupOrderAttribute>();

            var propertyGroups = (
                from property in dataType.GetProperties()
                where property.CanWrite
                let groupName = property.GetCustomAttribute<DisplayGroupAttribute>()?.Name ?? "Other"
                group property by groupName
                into g
                orderby groupOrders.FirstOrDefault(go => go.Name == g.Key)?.Order ?? int.MaxValue, g.Key
                select new
                {
                    Name = g.Key,
                    Properties = (
                        from pp in g
                        let ppo = pp.GetCustomAttribute<DisplayOrderAttribute>()?.Order ?? int.MaxValue
                        orderby ppo, pp.Name
                        select pp
                        ).ToArray()
                }).ToArray();

            foreach (var group in propertyGroups)
            {
                var groupSection = CreateGroupSection(group.Name);
                var grid = CreateGroupGrid();

                PopulateGroupGrid(grid, group.Properties, _data);

                if (grid.Children.Count > 0)
                {
                    groupSection.Content = grid;
                    spGroups.Children.Add(groupSection);
                }
            }

            _uiBuilt = true;
        }

        private static string GetPropertyName(MemberInfo memberInfo)
        {
            var nameAttr = memberInfo.GetCustomAttribute<DisplayNameAttribute>();
            return nameAttr != null ? nameAttr.DisplayName : StringHelper.SpaceOutPascal(memberInfo.Name);
        }

        private static string GetPropertyDescription(MemberInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
        }

        private Expander CreateGroupSection(string groupName)
        {
            var expander = new Expander
            {
                Header = groupName,
                OverridesDefaultStyle = true,
                Style = (Style)FindResource("ModelEditorGroupExpander"),
                Margin = new Thickness(0, 0, 0, 10),
                Tag = false
            };

            return expander;
        }

        private static Grid CreateGroupGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            return grid;
        }

        private void PopulateGroupGrid(Grid grid, PropertyInfo[] properties, object instance)
        {
            var rowIndex = -1;

            foreach (var property in properties)
            {
                var isReadonly = false;

                var displayModeAttr = property.GetCustomAttribute<DisplayModeAttribute>();
                if (displayModeAttr != null)
                {
                    if ((_editMode && displayModeAttr.EditMode == DisplayMode.Hidden) || displayModeAttr.CreateMode == DisplayMode.Hidden)
                    {
                        // Property should not be visible within the editor, so lets skip it.
                        continue;
                    }

                    // Determine if the property should be displayed as readonly.
                    isReadonly = (_editMode && displayModeAttr.CreateMode == DisplayMode.Readonly) || displayModeAttr.CreateMode == DisplayMode.Readonly;
                }

                rowIndex++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = GeneratePropertyLabel(property);
                Grid.SetRow(label, rowIndex);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);


                var field = isReadonly
                    ? GenerateTextBlock(property, instance)
                    : GeneratePropertyField(property, instance);
                Grid.SetRow(field, rowIndex);
                Grid.SetColumn(field, 1);
                grid.Children.Add(field);
            }
        }

        private static FrameworkElement GeneratePropertyLabel(MemberInfo memberInfo)
        {
            var label = new TextBlock
            {
                Name = $"lbl{memberInfo.Name}",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 10, 5),
                Text = $"{GetPropertyName(memberInfo)}:",
                ToolTip = GetPropertyDescription(memberInfo)
            };

            if (memberInfo.GetCustomAttribute<RequiredAttribute>() != null)
            {
                var starRun = new Run("*") { Foreground = Brushes.Red };
                label.Text = string.Empty;
                label.Inlines.Add(starRun);
                label.Inlines.Add($"{GetPropertyName(memberInfo)}:");
            }

            return label;
        }

        private FrameworkElement GeneratePropertyField(PropertyInfo propertyInfo, object instance)
        {
            if (propertyInfo.GetCustomAttribute<ReadOnlyAttribute>() != null)
            {
                return GenerateTextBlock(propertyInfo, instance);
            }

            var uiHintAttr = propertyInfo.GetCustomAttribute<UIHintAttribute>();
            if (!string.IsNullOrWhiteSpace(uiHintAttr?.UIHint))
            {
                if (uiHintAttr.UIHint == "AccentSelector")
                {
                    return GenerateColorSelectorField(
                        propertyInfo, 
                        instance, 
                        () => ThemeManager.Accents.Select(x => new ColorData
                            {
                                Name = x.Name,
                                ColorBrush = x.Resources["AccentColorBrush"] as Brush,
                                BorderColorBrush = Brushes.Transparent
                            }));
                }

                if (uiHintAttr.UIHint == "ThemeSelector")
                {
                    return GenerateColorSelectorField(
                        propertyInfo,
                        instance,
                        () => ThemeManager.AppThemes.Select(x => new ColorData
                        {
                            Name = x.Name.Replace("Base", string.Empty),
                            ColorBrush = x.Resources["WhiteColorBrush"] as Brush,
                            BorderColorBrush = x.Resources["BlackColorBrush"] as Brush
                        }));
                }

                if (uiHintAttr.UIHint == "CommandSelector")
                    return GenerateCommandSelectorField(propertyInfo, instance);

                if (uiHintAttr.UIHint == "BrowserSelector")
                    return GenerateBrowserSelectorField(propertyInfo, instance,
                        () => BrowserHelper.GetInstalledBrowsers().OrderBy(x => x));
            }

            if (propertyInfo.PropertyType == typeof(bool))
            {
                return GenerateToggleSwitch(propertyInfo, instance);
            }

            if (propertyInfo.PropertyType == typeof(int) ||
                propertyInfo.PropertyType == typeof(long) ||
                propertyInfo.PropertyType == typeof(float) ||
                propertyInfo.PropertyType == typeof(double) ||
                propertyInfo.PropertyType == typeof(decimal))
            {
                return GenerateNumericUpDown(propertyInfo, instance);
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                var fileSelectorAttr = propertyInfo.GetCustomAttribute<FileSelectorAttribute>();
                if (fileSelectorAttr != null)
                {
                    return GenerateFileSelector(propertyInfo, instance, fileSelectorAttr);
                }

                var folderSelectorAttr = propertyInfo.GetCustomAttribute<FolderSelectorAttribute>();
                if (folderSelectorAttr != null)
                {
                    return GenerateFolderSelector(propertyInfo, instance, folderSelectorAttr);
                }

                return GenerateTextBox(propertyInfo, instance);
            }

            if (propertyInfo.PropertyType == typeof(Guid))
            {
                return GenerateTextBox(propertyInfo, instance);
            }

            if (propertyInfo.PropertyType == typeof (HotKey))
            {
                return GenerateHotKeyBox(propertyInfo, instance);
            }

            return GenerateUnsupportedField(propertyInfo);
        }

        private static FrameworkElement GenerateUnsupportedField(MemberInfo memberInfo)
        {
            return new TextBlock
            {
                Name = $"lbl{memberInfo.Name}FieldUnsupported",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 5, 0, 5),
                Text = "Unsupported Data Type",
                ToolTip = GetPropertyDescription(memberInfo)
            };
        }

        private static FrameworkElement GenerateTextBlock(MemberInfo memberInfo, object instance)
        {
            var textBlock = new TextBlock
            {
                Name = $"txtBlk{memberInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                ToolTip = GetPropertyDescription(memberInfo)
            };

            textBlock.SetBinding(
                TextBlock.TextProperty,
                new Binding(memberInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.OneWay
                });

            return textBlock;
        }

        private static FrameworkElement GenerateToggleSwitch(MemberInfo memberInfo, object instance)
        {
            var toggleSwitch = new ToggleSwitch
            {
                Name = $"chk{memberInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                OffLabel = string.Empty,
                OnLabel = string.Empty
            };

            toggleSwitch.SetBinding(ToggleSwitch.IsCheckedProperty,
                new Binding(memberInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return toggleSwitch;
        }

        private static FrameworkElement GenerateTextBox(MemberInfo memberInfo, object instance)
        {
            var textBox = new TextBox
            {
                Name = $"txt{memberInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5)
            };

            TextBoxHelper.SetClearTextButton(textBox, true);

            textBox.SetBinding(TextBox.TextProperty,
                new Binding(memberInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return textBox;
        }

        private FrameworkElement GenerateFileSelector(PropertyInfo propertyInfo, object instance, FileSelectorAttribute fileSelectorAttr)
        {
            var nestedGrid = new Grid();
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var fileDialog = new OpenFileDialog
            {
                DefaultExt = fileSelectorAttr.DefaultExt,
                Filter = fileSelectorAttr.Filter,
                CheckFileExists = fileSelectorAttr.CheckFileExists,
                InitialDirectory = fileSelectorAttr.InitialDirectory,
                RestoreDirectory = fileSelectorAttr.RestoreDirectory,
                Title = fileSelectorAttr.Title
            };
            OpenFileDialogs.Add(fileDialog);

            var textBox = new TextBox
            {
                Name = $"txt{propertyInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                Style = (Style) FindResource("ButtonCommandMetroTextBox")
            };

            TextBoxHelper.SetButtonTemplate(textBox, (ControlTemplate) FindResource("OpenFileMetroTextBoxButtonTemplate"));
            TextBoxHelper.SetButtonCommand(textBox, new RelayCommand(() =>
            {
                var result = fileDialog.ShowDialog();

                if (result == true)
                {
                    propertyInfo.SetValue(instance, fileDialog.FileName);
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
                }
            }));

            textBox.SetBinding(TextBox.TextProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            Grid.SetColumn(textBox, 0);
            nestedGrid.Children.Add(textBox);

            return nestedGrid;
        }

        private FrameworkElement GenerateFolderSelector(PropertyInfo propertyInfo, object instance, FolderSelectorAttribute folderSelectorAttr)
        {
            var nestedGrid = new Grid();
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var folderDialog = new FolderBrowserDialog
            {
                Description = folderSelectorAttr.Description,
                RootFolder = folderSelectorAttr.RootFolder,
                ShowNewFolderButton = folderSelectorAttr.ShowNewFolderButton
            };
            OpenFolderDialogs.Add(folderDialog);

            var textBox = new TextBox
            {
                Name = $"txt{propertyInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                Style = (Style)FindResource("ButtonCommandMetroTextBox")
            };

            TextBoxHelper.SetButtonTemplate(textBox, (ControlTemplate)FindResource("OpenFileMetroTextBoxButtonTemplate"));
            TextBoxHelper.SetButtonCommand(textBox, new RelayCommand(() =>
            {
                var result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    propertyInfo.SetValue(instance, folderDialog.SelectedPath);
                    textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
                }
            }));

            textBox.SetBinding(TextBox.TextProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            Grid.SetColumn(textBox, 0);
            nestedGrid.Children.Add(textBox);

            return nestedGrid;
        }

        private static FrameworkElement GenerateNumericUpDown(PropertyInfo propertyInfo, object instance)
        {
            var numericUpDown = new NumericUpDown
            {
                Name = $"nud{propertyInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                TextAlignment = TextAlignment.Left,
                HasDecimals = true,
                Minimum = 0,
                Maximum = double.MaxValue
            };

            if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long))
            {
                // Don't allow decimal in field if the data type doesn't support decimal places.
                numericUpDown.HasDecimals = false;
            }

            var rangeAttr = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                try
                {
                    if (rangeAttr.Minimum != null)
                        numericUpDown.Minimum = (double)rangeAttr.Minimum;
                }
                catch (InvalidCastException)
                {
                    // If cast fails, just use default minimum value.
                }

                try
                {
                    if (rangeAttr.Maximum != null)
                        numericUpDown.Maximum = (double)rangeAttr.Maximum;
                }
                catch (InvalidCastException)
                {
                    // If cast fails, just use default maximum value.
                }
            }

            numericUpDown.SetBinding(NumericUpDown.ValueProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return numericUpDown;
        }

        private static FrameworkElement GenerateHotKeyBox(MemberInfo memberInfo, object instance)
        {
            var hotKeyBox = new HotKeyBox
            {
                Name = $"hkb{memberInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                AreModifierKeysRequired = true
            };

            ControlsHelper.SetContentCharacterCasing(hotKeyBox, CharacterCasing.Upper);

            hotKeyBox.SetBinding(HotKeyBox.HotKeyProperty,
                new Binding(memberInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return hotKeyBox;
        }

        private FrameworkElement GenerateColorSelectorField(PropertyInfo propertyInfo, object instance, Func<IEnumerable<ColorData>> getColors)
        {
            var comboBox = new ComboBox
            {
                Name = $"cmb{propertyInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                SelectedValuePath = "Name",
                ItemsSource = getColors(),
                Style = (Style) FindResource("ColorSelectorComboBox")
            };

            comboBox.SetBinding(Selector.SelectedValueProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return comboBox;
        }

        private FrameworkElement GenerateCommandSelectorField(PropertyInfo propertyInfo, object instance)
        {
            var commandManager = DependencyResolver.Current.Resolve<IHotCommandManager>();
            var allCommands = commandManager.GetAll(excludeInvariant: false, includeDisabled: true)
                .Select(c => new
                {
                    ModuleName =
                        StringHelper.SpaceOutPascal(c.GetType()
                            .Assembly.GetName()
                            .Name.Replace("ShaneYu.HotCommander", "Core")
                            .Replace("HotCommander", "UI")
                            .Replace(".", " ")),
                    TypeName = StringHelper.SpaceOutPascal(c.GetType().Name.Replace("Command", "")),
                    Command = c
                }).ToArray();

            var listCollectionView = new ListCollectionView(allCommands);
            listCollectionView.GroupDescriptions?.Add(new PropertyGroupDescription("ModuleName"));
            listCollectionView.GroupDescriptions?.Add(new PropertyGroupDescription("TypeName"));
            listCollectionView.SortDescriptions.Add(new SortDescription("ModuleName", ListSortDirection.Ascending));
            listCollectionView.SortDescriptions.Add(new SortDescription("TypeName", ListSortDirection.Ascending));
            listCollectionView.SortDescriptions.Add(new SortDescription("Command.Configuration.Name", ListSortDirection.Ascending));

            var comboBox = new ComboBox
            {
                Name = $"cmb{propertyInfo.Name}Field",
                ItemsSource = listCollectionView,
                DisplayMemberPath = "Command.Configuration.Name",
                SelectedValuePath = "Command.Configuration.Id",
                Margin = new Thickness(0, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                GroupStyle =
                {
                    new GroupStyle { ContainerStyle = (Style) FindResource("groupStyleLevel0") },
                    new GroupStyle { ContainerStyle = (Style) FindResource("groupStyleLevel1") },
                    new GroupStyle { ContainerStyle = (Style) FindResource("groupStyleLevel2") }
                }
            };

            comboBox.SetBinding(Selector.SelectedValueProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            return comboBox;
        }

        private FrameworkElement GenerateBrowserSelectorField(PropertyInfo propertyInfo, object instance, Func<IEnumerable<BrowserHelper.BrowserDetail>> getBrowsers)
        {
            var browsers = getBrowsers().ToArray();

            var comboBox = new ComboBox
            {
                Name = $"cmb{propertyInfo.Name}Field",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5),
                ItemsSource = browsers.Select(x => x.Name).OrderBy(x => x)
            };

            comboBox.SetBinding(Selector.SelectedValueProperty,
                new Binding(propertyInfo.Name)
                {
                    Source = instance,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnDataErrors = true,
                    NotifyOnValidationError = true,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

            // Attempt to set default selection based on OS default browser...
            var currentValue = (string) propertyInfo.GetValue(instance);

            if (string.IsNullOrWhiteSpace(currentValue))
            {
                var defaultBrowser = browsers.FirstOrDefault(x => x.IsDefault);

                if (defaultBrowser == null)
                    defaultBrowser = browsers.FirstOrDefault();
                
                if (defaultBrowser != null)
                    propertyInfo.SetValue(instance, defaultBrowser.Name);
            }

            return comboBox;
        }

        #endregion

        #region Public Methods

        public void SetData<T>()
            where T : class, INotifyPropertyChanged, new()
        {
            RemoveEventHandlers();
            RemoveDataPropertyChangedHandler();

            _data = new T();
            _dataClone = new T();
            _editMode = false;
            IsChanged = false;

            AttachDataPropertyChangedHandler();
            BuildDynamicUI();
        }

        public void SetData<T>(T data)
            where T : class, INotifyPropertyChanged
        {
            RemoveEventHandlers();
            RemoveDataPropertyChangedHandler();

            _data = data;
            _dataClone = CloneData(_data);
            _editMode = true;
            IsChanged = false;

            AttachDataPropertyChangedHandler();
            BuildDynamicUI();
        }

        public void SetData(Type dataType)
        {
            if (!typeof (INotifyPropertyChanged).IsAssignableFrom(dataType))
            {
                throw new ArgumentException(@"The model editor only supports types that implements 'INotifyPropertyChanged'.", nameof(dataType));
            }

            RemoveEventHandlers();
            RemoveDataPropertyChangedHandler();

            _data = (INotifyPropertyChanged) Activator.CreateInstance(dataType);
            _dataClone = CloneData(_data);
            _editMode = false;
            IsChanged = false;

            AttachDataPropertyChangedHandler();
            BuildDynamicUI();
        }

        /// <summary>
        /// Accepts the changes and performs the commit function.
        /// </summary>
        public void AcceptChanges()
        {
            if (ValidationHelper.ValidateObject(_data))
            {
                var acceptingArgs = new ModelEditorAcceptingEventArgs();
                Accepting?.Invoke(this, acceptingArgs);

                if (!acceptingArgs.Reject)
                {
                    _dataClone = CloneData(_data);
                    IsChanged = false;
                    Accepted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Rejects the changes.
        /// </summary>
        public void RejectChanges()
        {
            PropertyCopier.Copy(_dataClone, _data);
            IsChanged = false;
            Rejected?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Model Editor Accepting Event Arguments
        /// </summary>
        public sealed class ModelEditorAcceptingEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets whether to reject the acceptance of the changes.
            /// This does not reset the changed values, it just aborts acceptance of them.
            /// </summary>
            public bool Reject { get; set; }
        }

        #endregion
    }
}
