using System;
using System.Runtime.InteropServices;
using System.Windows;

using ShaneYu.HotCommander.UI.WPF.Helpers;

namespace ShaneYu.HotCommander.UI.WPF.Extensions
{
    public static class WindowExtensions
    {
        #region Public Methods

        public static void ShowOnActiveMonitor(this Window window,
            Action<Window, Win32API.W32MonitorInfo> computeAction = null)
        {
            if (computeAction == null)
                computeAction = DefaultComputeAction;

            var oldStartupLocation = window.WindowStartupLocation;
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            computeAction.Invoke(window, GetActiveMonitorInfo());

            window.Show();
            window.WindowStartupLocation = oldStartupLocation;
        }

        public static bool ActivateOnActiveMonitor(this Window window,
            Action<Window, Win32API.W32MonitorInfo> computeAction = null)
        {
            if (computeAction == null)
                computeAction = DefaultComputeAction;

            computeAction.Invoke(window, GetActiveMonitorInfo());

            return window.Activate();
        }

        #endregion

        #region Private Methods

        private static void DefaultComputeAction(Window window, Win32API.W32MonitorInfo monitorInfo)
        {
            var windowStateAfterActivation = window.WindowState;

            if (windowStateAfterActivation == WindowState.Minimized)
                windowStateAfterActivation = WindowState.Normal;

            window.WindowState = WindowState.Normal;

            var monitorRect = monitorInfo.WorkArea;
            var monitorWidth = monitorRect.Right - monitorRect.Left;
            var monitorHeight = monitorRect.Bottom - monitorRect.Top;
            var monitorWidth15Percent = monitorWidth * .15;
            var monitorHeight15Percent = monitorHeight * .15;

            window.Top = monitorRect.Top + monitorHeight15Percent;
            window.Left = monitorRect.Left + monitorWidth15Percent;

            window.Width = monitorWidth - (monitorWidth15Percent * 2);
            window.Height = monitorHeight - (monitorHeight15Percent * 2);

            window.WindowState = windowStateAfterActivation;
        }

        private static Win32API.W32Point GetCursorPosition()
        {
            var cursorPoint = new Win32API.W32Point();

            if (!Win32API.GetCursorPos(ref cursorPoint))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            return cursorPoint;
        }

        private static Win32API.W32MonitorInfo GetActiveMonitorInfo()
        {
            var cursorPoint = GetCursorPosition();
            var monitorHandle = Win32API.MonitorFromPoint(cursorPoint, 0x00000002); // 0x00000002: Return nearest monitor if cursorPos is not contained in any monitor.
            var monitorInfo = new Win32API.W32MonitorInfo { Size = Marshal.SizeOf(typeof(Win32API.W32MonitorInfo)) };

            if (!Win32API.GetMonitorInfo(monitorHandle, ref monitorInfo))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            return monitorInfo;
        }

        #endregion
    }
}
