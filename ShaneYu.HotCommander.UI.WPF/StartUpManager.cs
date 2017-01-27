using System;
using System.Reflection;

using Microsoft.Win32.TaskScheduler;

namespace ShaneYu.HotCommander.UI.WPF
{
    public static class StartUpManager
    {
        private const string ApplicationName = "Launch HotCommander";

        public static bool IsApplicationRegisteredToStartupWithWindows()
        {
            return TaskService.Instance.FindTask(ApplicationName) != null;
        }

        public static void AddApplicationToCurrentUserStartup()
        {
            var taskDef = TaskService.Instance.NewTask();

            taskDef.RegistrationInfo.Description = "Launches HotCommander on (any) user logon.";
            taskDef.Triggers.Add(new LogonTrigger());
            taskDef.Actions.Add(new ExecAction(Assembly.GetExecutingAssembly().Location));

            taskDef.Principal.RunLevel = TaskRunLevel.Highest;

            taskDef.Settings.RestartCount = 3;
            taskDef.Settings.RestartInterval = TimeSpan.FromMinutes(1);
            taskDef.Settings.DisallowStartIfOnBatteries = false;
            taskDef.Settings.StopIfGoingOnBatteries = false;

            TaskService.Instance.RootFolder.RegisterTaskDefinition(ApplicationName, taskDef);
        }

        public static void RemoveApplicationFromCurrentUserStartup()
        {
            TaskService.Instance.RootFolder.DeleteTask(ApplicationName, false);
        }
    }
}
