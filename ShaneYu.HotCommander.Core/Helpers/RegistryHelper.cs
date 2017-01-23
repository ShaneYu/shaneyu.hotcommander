using Microsoft.Win32;

namespace ShaneYu.HotCommander.Helpers
{
    public static class RegistryHelper
    {
        public static object GetValue(RegistryKey regKey, string subKeyName, string name = null, object defaultValue = null)
        {
            if (regKey == null)
            {
                return defaultValue;
            }

            RegistryKey registryKey = null;

            try
            {
                registryKey = regKey.OpenSubKey(subKeyName, false);

                if (registryKey != null)
                {
                    return registryKey.GetValue(name, defaultValue);
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
            finally
            {
                registryKey?.Close();
            }
        }
    }
}
