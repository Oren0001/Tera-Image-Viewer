using System;
using System.Globalization;
using System.Windows.Data;

namespace TeraImageViewer.Converters {
    public class TimestampConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) {
                return "N/A";
            }

            if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTime)) {
                return dateTime.ToLocalTime().ToString("MMM dd, yyyy h:mm:ss tt", CultureInfo.CurrentCulture);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
