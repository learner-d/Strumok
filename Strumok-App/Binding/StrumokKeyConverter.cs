using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace Strumok_App.Binding
{
    public class StrumokKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        readonly Regex regex = new Regex(@"([\dA-F]{1,4})-([\dA-F]{1,4})-([\dA-F]{1,4})-([\dA-F]{1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public ulong[] ConvertBack(string[] keyParts)
        {
            if (keyParts == null)
                throw new ArgumentNullException(nameof(keyParts));
            if (keyParts.Length != 4 && keyParts.Length != 8)
                throw new ArgumentOutOfRangeException(nameof(keyParts));

            ulong[] result = new ulong[keyParts.Length];

            for (int i = 0; i < keyParts.Length; i++)
            {
                Match match = regex.Match(keyParts[i]);
                if (!match.Success)
                    return null;

                ulong value = 0;
                for (int j = 1; j < match.Groups.Count; j++)
                {
                    ushort val = ushort.Parse(match.Groups[j].Value, NumberStyles.HexNumber);
                    value |= ((ulong)val << 64 - (j+1) * 16);
                }

                result[i] = value;
            }
            return result;
        }
    }
}
