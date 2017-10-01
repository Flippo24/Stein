﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TempManager.Converters
{
    public class StringNotNullOrEmptyToVisibilityConverter
        : IValueConverter
    {
        public static StringNotNullOrEmptyToVisibilityConverter Instance = new StringNotNullOrEmptyToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(value as string))
                return Visibility.Visible;

            switch (parameter as string)
            {
                case "Hidden": return Visibility.Hidden;
                case "Visible": return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
