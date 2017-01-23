using System;
using System.Globalization;
using System.Windows.Data;

namespace ShaneYu.HotCommander.UI.WPF.Converters
{
    public class BooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof (bool))
            {
                throw new InvalidOperationException("The value must be of type boolean.");
            }

            var paramStr = parameter as string;

            if (!string.IsNullOrWhiteSpace(paramStr) && paramStr.Contains("|"))
            {
                var paramParts = paramStr.Split('|');

                if (paramParts.Length == 2)
                {
                    return (bool) value ? paramParts[0] : paramParts[1];
                }
            }

            return (bool) value ? "True" : "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramStr = parameter as string;

            if (!string.IsNullOrWhiteSpace(paramStr) && paramStr.Contains("|"))
            {
                var paramParts = paramStr.Split('|');

                if (paramParts.Length == 2)
                {
                    if ((string) value == paramParts[0])
                    {
                        return true;
                    }

                    if ((string)value == paramParts[1])
                    {
                        return false;
                    }

                    throw new InvalidOperationException("Invalid value, cannot resolve to true or false.");
                }
            }

            return (string) value == "True";
        }
    }
}
