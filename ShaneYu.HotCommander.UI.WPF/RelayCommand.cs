using System;
using System.Windows.Input;

namespace ShaneYu.HotCommander.UI.WPF
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private Action<object> _execute;
        private Predicate<object> _canExecute;

        #endregion

        #region Events

        private event EventHandler CanExecuteChangedInternal;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                System.Windows.Input.CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                System.Windows.Input.CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        #endregion

        #region Constructor

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute ?? DefaultCanExecute;
        }

        public RelayCommand(Action execute, Predicate<object> canExecute)
            : this(obj => execute(), canExecute)
        {
        }

        public RelayCommand(Action execute)
            : this(obj => execute(), DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = DefaultCanExecute;

            if (canExecute != null)
                _canExecute = obj => canExecute();
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(obj => execute(), canExecute)
        {
        }

        #endregion

        #region Private Methods

        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }

        #endregion

        #region Public Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute != null && _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            var handler = CanExecuteChangedInternal;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public void Destroy()
        {
            _canExecute = _ => false;
            _execute = _ => { };
        }

        #endregion
    }
}
