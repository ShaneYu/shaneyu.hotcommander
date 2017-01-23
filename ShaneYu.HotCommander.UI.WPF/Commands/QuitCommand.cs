using System;
using System.ComponentModel;
using System.Windows;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander.UI.WPF.Commands
{
    [InternalHotCommand]
    internal sealed class QuitCommand : HotCommandBase<QuitConfiguration>
    {
        #region Constructors

        public QuitCommand()
            : base(new QuitConfiguration())
        {
        }

        #endregion

        #region Overrides

        public override void Execute()
        {
            Application.Current.Shutdown(0);
        }

        #endregion
    }

    internal sealed class QuitConfiguration : IHotCommandConfiguration
    {
        #region Properties

        public Guid Id
        {
            get { return new Guid("48E0E87D-CCAE-46FF-8F21-E9A356F471F1"); }
            set { }
        }

        public string Name
        {
            get { return "Quit"; }
            set { }
        }

        public string Description
        {
            get { return "Terminates the application closing the HotCommander."; }
            set { }
        }

        public bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
