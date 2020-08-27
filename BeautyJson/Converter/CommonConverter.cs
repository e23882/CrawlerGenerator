using System;
using System.Globalization;
using System.Windows.Data;

namespace BeautyJson.Converter
{
    public class CommonConverter
    {
        
    }

    public class StringNullOrEmptyToBoolConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inputParameter = (value as string);
            if (inputParameter is null)
                return false;
            if (string.IsNullOrEmpty(inputParameter))
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}