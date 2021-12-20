using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StrumokApp.Binding
{
    internal class NullToVisibillityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object argument, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new ArgumentException(nameof(targetType));
            return value != null? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object argument, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
