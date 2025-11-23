using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace TeraImageViewer.Converters {
    public class HistogramHeightConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values == null || values.Length < 2) {
                return 0.0;
            }

            if (values[0] is int value && values[1] is double containerHeight) {
                var maxValue = 255.0;
                var height = (value / maxValue) * (containerHeight - 20);
                return Math.Max(height, 1.0);
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
