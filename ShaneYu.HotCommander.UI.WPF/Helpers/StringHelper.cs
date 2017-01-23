using System.Text.RegularExpressions;

namespace ShaneYu.HotCommander.UI.WPF.Helpers
{
    public static class StringHelper
    {
        public static string SpaceOutPascal(string pascalString, string separator = " ")
        {
            return Regex.Replace(pascalString, "([a-z]+)([A-Z]+)", "$1" + separator + "$2", RegexOptions.Compiled);
        }
    }
}
