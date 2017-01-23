using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using ShaneYu.HotCommander.UI.WPF.Helpers;

namespace ShaneYu.HotCommander.UI.WPF.Converters
{
    public class TypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StringHelper.SpaceOutPascal((value as Type)?.Name.Replace("Command", string.Empty));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
