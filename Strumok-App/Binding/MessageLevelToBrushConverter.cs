using StrumokApp.Data;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StrumokApp.Binding
{
    internal class MessageLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object argument, CultureInfo culture)
        {
            if (!(value is eMessageLevel messageLevel))
                throw new ArgumentException(nameof(value));
            if (targetType != typeof(Brush))
                throw new ArgumentException(nameof(targetType));
            switch (messageLevel)
            {
                case eMessageLevel.Info:
                    return Brushes.Blue;
                case eMessageLevel.Success:
                    return Brushes.Green;
                case eMessageLevel.Warning:
                    return Brushes.Orange;
                case eMessageLevel.Error:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object argument, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}