using System;
using System.Globalization;
using System.Windows.Data;

namespace DrivingCertificateApp.Converters
{
    public class BooleanToReadStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRead = (bool)value;
            return isRead ? "Đã đọc" : "Chưa đọc";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}