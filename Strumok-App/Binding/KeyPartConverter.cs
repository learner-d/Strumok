using System;
using System.Windows.Data;
using System.Globalization;
using System.Linq;


namespace StrumokApp.Binding
{
    internal class KeyPartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (targetType != typeof(string))
                throw new ArgumentException(nameof(targetType));
            
            if (!(value is ulong ulValue))
                throw new ArgumentException(nameof(value));

            byte[] bArrValue = BitConverter.GetBytes(ulValue);
            return $"{bArrValue[7]:X2}{bArrValue[6]:X2}-{bArrValue[5]:X2}{bArrValue[4]:X2}-{bArrValue[3]:X2}{bArrValue[2]:X2}-{bArrValue[1]:X2}{bArrValue[0]:X2}";
        }

        const int KEY_LEN = 8;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (targetType != typeof(ulong))
                throw new ArgumentException(nameof(targetType));

            if (!(value is string strValue))
                throw new ArgumentException(nameof(value));

            byte[] byteArrValue = new byte[KEY_LEN];

            strValue = strValue.Replace("-", "");
            if (strValue.Length % 2 != 0)
                strValue += "0";

            for (int i = 0; i < strValue.Length && i < KEY_LEN * 2; i+=2)
                byteArrValue[i / 2] = byte.Parse(strValue.Substring(i, 2), NumberStyles.HexNumber);
            return BitConverter.ToUInt64(byteArrValue, 0);
        }
    }
}
