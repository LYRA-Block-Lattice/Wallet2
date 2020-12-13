using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Wallet2.Shared.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"{value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Decimal.TryParse((string)value, out decimal result))
                return result;
            else
                return 0m;
        }
    }
}
