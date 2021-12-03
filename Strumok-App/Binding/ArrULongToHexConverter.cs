using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StrumokApp.Binding
{
    internal class ArrULongToHexConverter : IValueConverter
    {
        // ulong[] to string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (targetType != typeof(string))
                throw new ArgumentException(nameof(targetType));
            if (!(value is ulong[] valULArray))
                throw new ArgumentException(nameof(value));

            return string.Join("-", valULArray.Select(valUL => valUL.ToString("X16")));
        }

        // string to ulong[]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
