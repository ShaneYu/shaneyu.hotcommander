using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Win32;

namespace ShaneYu.HotCommander.Helpers
{
    public static class BrowserHelper
    {
        private static bool IsDefault(string executablePath)
        {
            var defaultCmd = (string) RegistryHelper.GetValue(Registry.ClassesRoot, @"HTTP\shell\open\command");

            if (!string.IsNullOrWhiteSpace(defaultCmd))
            {
                return defaultCmd.ToLowerInvariant()
                    .Contains(executablePath.ToLowerInvariant());
            }

            return false;
        }

        public static IEnumerable<BrowserDetail> GetInstalledBrowsers()
        {
            var browsers = new List<BrowserDetail>();
            RegistryKey browsersKey = null;

            try
            {
                browsersKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet", false);

                if (browsersKey == null)
                    browsersKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet", false);

                if (browsersKey != null)
                {
                    foreach (var subKeyName in browsersKey.GetSubKeyNames())
                    {
                        RegistryKey subKey = null;

                        try
                        {
                            subKey = browsersKey.OpenSubKey(subKeyName, false);
                            var name = (string) subKey?.GetValue(null, null);

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                var executablePath = (string) RegistryHelper.GetValue(subKey, @"shell\open\command");

                                if (!string.IsNullOrWhiteSpace(executablePath))
                                {
                                    browsers.Add(new BrowserDetail(name, executablePath, IsDefault(executablePath)));
                                }
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            subKey?.Close();
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                browsersKey?.Close();
            }

            return browsers;
        }

        public static BrowserDetail GetBrowserOrDefault(string name)
        {
            var installedBrowsers = GetInstalledBrowsers().ToArray();
            var browser = installedBrowsers.FirstOrDefault(b => 
                b.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (browser != null)
            {
                return browser;
            }

            return installedBrowsers.FirstOrDefault(b => b.IsDefault);
        }

        public class BrowserDetail : IComparable
        {
            public string Name { get; }

            public string ExecutablePath { get; }

            public bool IsDefault { get; }

            public BrowserDetail(string name, string executablePath, bool isDefault = false)
            {
                Name = name;
                ExecutablePath = executablePath;
                IsDefault = isDefault;
            }

            public int CompareTo(object obj)
            {
                var other = obj as BrowserDetail;

                if (other == null)
                    return 1;

                return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
