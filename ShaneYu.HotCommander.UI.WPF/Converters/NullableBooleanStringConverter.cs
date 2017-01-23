using System;
using System.Globalization;
using System.Windows.Data;

namespace ShaneYu.HotCommander.UI.WPF.Converters
{
    public class NullableBooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() != typeof (bool))
            {
                throw new InvalidOperationException("The value must be of type boolean.");
            }

            var boolValue = value as bool?;
            var paramStr = parameter as string;

            if (!string.IsNullOrWhiteSpace(paramStr) && paramStr.Contains("|"))
            {
                var paramParts = paramStr.Split('|');

                if (paramParts.Length == 3)
                {
                    return boolValue.HasValue ? (boolValue.Value ? paramParts[0] : paramParts[1]) : paramParts[2];
                }
            }

            return boolValue.HasValue ? (boolValue.Value ? "True" : "False") : "Null";
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

                    if ((string) value == paramParts[2])
                    {
                        return null;
                    }

                    throw new InvalidOperationException("Invalid value, cannot resolve to true, false or null.");
                }
            }

            if ((string) value == "Null")
            {
                return null;
            }

            return (string) value == "True";
        }
    }
}
