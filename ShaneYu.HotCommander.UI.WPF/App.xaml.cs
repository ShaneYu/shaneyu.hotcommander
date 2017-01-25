using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

using Autofac;

using MahApps.Metro;

using NHotkey;
using NHotkey.Wpf;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.IoC;
using ShaneYu.HotCommander.Storage;
using ShaneYu.HotCommander.UI.WPF.Commands;
using ShaneYu.HotCommander.UI.WPF.Extensions;
using ShaneYu.HotCommander.UI.WPF.Logging;
using ShaneYu.HotCommander.UI.WPF.Models;
using ShaneYu.HotCommander.UI.WPF.Settings;
using ShaneYu.HotCommander.UI.WPF.Storage;
using ShaneYu.HotCommander.UI.WPF.Validation;
using ShaneYu.HotCommander.UI.WPF.Windows;
using ShaneYu.HotCommander.Validation;

namespace ShaneYu.HotCommander.UI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        #region Application Entry

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(UniqueIdentity))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #endregion

        #region Fields

        private static string _uniqueIdentity;
        private ApplicationSettingsProvider _applicationSettingsProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current application instance
        /// </summary>
        public new static App Current => (App) Application.Current;

        /// <summary>
        /// Gets the unique identity for the application.
        /// This identity is used to determine single instances.
        /// </summary>
        public static string UniqueIdentity
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_uniqueIdentity))
                {
                    _uniqueIdentity = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>()?.Value;

                    if (string.IsNullOrWhiteSpace(_uniqueIdentity))
                    {
                        _uniqueIdentity = Guid.NewGuid().ToString();
                    }
                }

                return _uniqueIdentity;
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The directory in which all application data will be stored.
        /// This includes application settings and custom command data.
        /// </summary>
        public string DataDirectory => "%APPDATA%\\HotCommander";

        /// <summary>
        /// The name for the application settings.
        /// </summary>
        public string ApplicationSettingsName => "Application";

        /// <summary>
        /// The name for registering the hotkey.
        /// </summary>
        public string HotKeyName => "HotCommander:Activate";

        public ApplicationSettingsProvider ApplicationSettingsProvider => 
            _applicationSettingsProvider ?? (_applicationSettingsProvider = DependencyResolver.Current.Resolve<ApplicationSettingsProvider>());

        #endregion

        #region Event Handlers

        private static void HotKey_Pressed(object sender, HotkeyEventArgs e)
        {
            DependencyResolver.Current.Resolve<CommandBar>().ShowAndFocus();
        }

        #endregion

        #region Private Methods

        private void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterModule<LoggerInjectionModule>();
            builder.RegisterGeneric(typeof(SettingsStorageStrategy<>)).AsImplementedInterfaces();
            builder.RegisterType<HotCommandManagerStorageStrategy>().As<ICollectionStorageStrategy<Guid, IHotCommand<IHotCommandConfiguration>>>();
            builder.RegisterType<ApplicationSettingsProvider>().AsSelf().SingleInstance();
            builder.RegisterType<HotCommandManager>().As<IHotCommandManager>().SingleInstance();

            builder.RegisterType<CommandCenterViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<CommandBarViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<CommandBar>().AsSelf().SingleInstance();
            builder.RegisterType<CommandCenter>().AsSelf();

            builder.RegisterType<FileExistsValidator>().As<IFileExistsValidator>();
            builder.RegisterType<DirectoryExistsValidator>().As<IDirectoryExistsValidator>();
        }

        private void RegisterInternalCommands(IHotCommandManager commandManager)
        {
            commandManager.Register(new QuitCommand());
            commandManager.Register(new ReloadCommand());
            commandManager.Register(new ConfigureCommand());
            commandManager.Register(new SetThemeBaseCommand());
            commandManager.Register(new SetThemeAccentCommand());
            commandManager.Register(new GithubCommand());
            commandManager.Register(new IssuesCommand());
            commandManager.Register(new ReleasesCommand());
        }

        private void UnregisterHotKey()
        {
            HotkeyManager.Current.Remove(HotKeyName);
        }

        #endregion

        #region Public Methods

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }

            ((CommandBar) MainWindow).ShowOnActiveMonitor();
            return true;
        }

        /// <summary>
        /// Apply a desired theme to the application
        /// </summary>
        /// <param name="accentColor">The desired accent color</param>
        /// <param name="baseTheme">The desired base theme</param>
        public void SetTheme(string accentColor, string baseTheme)
        {
            var accent = ThemeManager.GetAccent(accentColor);
            var theme = ThemeManager.GetAppTheme("Base" + baseTheme);

            ThemeManager.ChangeAppStyle(this, accent, theme);
        }

        /// <summary>
        /// Apply desired hotkey to application
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="modifierKeys">The modifier keys</param>
        public void SetHotKey(Key key, ModifierKeys modifierKeys)
        {
            HotkeyManager.Current.AddOrReplace(HotKeyName, key, modifierKeys, HotKey_Pressed);
        }

        /// <summary>
        /// Reload and apply theme from settings.
        /// </summary>
        public void ReloadThemeFromSettings()
        {
            if (ApplicationSettingsProvider == null) return;

            SetTheme(
                ApplicationSettingsProvider.Settings.AccentColor,
                ApplicationSettingsProvider.Settings.ThemeBase
            );
        }

        /// <summary>
        /// Reload and register hot key from settings.
        /// </summary>
        public void ReloadAndRegisterHotKeyFromSettings()
        {
            if (ApplicationSettingsProvider == null) return;

            SetHotKey(
                ApplicationSettingsProvider.Settings.Key,
                ApplicationSettingsProvider.Settings.ModifierKeys
            );
        }

        #endregion

        #region Overrides

        protected override async void OnStartup(StartupEventArgs e)
        {
            DependencyResolver.Initialize(RegisterTypes, RegisterInternalCommands);
            base.OnStartup(e);

            ReloadThemeFromSettings();
            ReloadAndRegisterHotKeyFromSettings();

            if (ApplicationSettingsProvider != null)
            {
                ApplicationSettingsProvider.Saved += (sender, args) =>
                {
                    SetTheme(args.Settings.AccentColor, args.Settings.ThemeBase);
                    SetHotKey(args.Settings.Key, args.Settings.ModifierKeys);
                };
            }

            // Showing first in order for layout and postioning to be correct when shown next time; I
            // believe this is due to calculated values; if hidden first when we show it, it may not be 
            // correctly centered on the screen.
            MainWindow = DependencyResolver.Current.Resolve<CommandBar>();
            MainWindow.Visibility = Visibility.Hidden;
            MainWindow.Hide();

            // Since the application has just started up, load in all of the commands from storage.
            var commandManager = DependencyResolver.Current.Resolve<IHotCommandManager>();
            await commandManager.LoadAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            UnregisterHotKey();
            base.OnExit(e);
        }

        #endregion
    }
}
