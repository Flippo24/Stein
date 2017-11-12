﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfBase.Converters
{
    public class ValueNotNullToBoolConverter
        : IValueConverter
    {
        public static ValueNotNullToBoolConverter Instance = new ValueNotNullToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
