using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Virtualization.Converters
{
    public class MarginPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var height = values[0];
            ContentPresenter p = (ContentPresenter)values[0];
            var r = VisualTreeHelper.GetClip(p);
            var width = values[1];
            var margin = (Thickness)values[2];

            return new Thickness();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
