using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TeraImageViewer.Converters {
    public class Base64ToImageConverter : IValueConverter {
        private static BitmapImage _placeholderImage;

        private BitmapImage GetPlaceholderImage() {
            if (_placeholderImage == null) {
                try {
                    _placeholderImage = new BitmapImage();
                    _placeholderImage.BeginInit();
                    _placeholderImage.UriSource = new Uri("pack://application:,,,/Assets/NoImagePlaceholder.jpg");
                    _placeholderImage.CacheOption = BitmapCacheOption.OnLoad;
                    _placeholderImage.EndInit();
                    _placeholderImage.Freeze();
                } catch {
                    return null;
                }
            }
            return _placeholderImage;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string base64String && !string.IsNullOrEmpty(base64String)) {
                try {
                    byte[] imageBytes = System.Convert.FromBase64String(base64String);

                    using (var ms = new MemoryStream(imageBytes)) {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                } catch {
                    return GetPlaceholderImage();
                }
            }
            return GetPlaceholderImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
