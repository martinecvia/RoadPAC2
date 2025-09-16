using System.Globalization;
using System.Windows.Media;
using System;
using System.Windows;
using System.Windows.Data;

namespace Shared.Windows.Models
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class StringToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorName)
                return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorName));
            return System.Windows.Media.Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}