using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace StrumokApp.Binding
{
    public class StrumokKeyConverter : IValueConverter
    {
        private static readonly KeyPartConverter _s_keyPartConverter = new KeyPartConverter();
        // ulong[] to string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (targetType != typeof(string))
                throw new ArgumentException(nameof(targetType));
            if (!(value is ulong[] valULArray))
                throw new ArgumentException(nameof(value));

            return string.Join("-", valULArray.Select(valUL => _s_keyPartConverter.Convert(valUL, targetType, parameter, culture)));
        }

        // string to ulong[]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
