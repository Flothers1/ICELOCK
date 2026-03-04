using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace i_freeze.View
{
    public class WidthSubtractConverter : IValueConverter
    {
        // parameter expected as a number of pixels to subtract (e.g. widths of other columns + margins)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double total)
            {
                double sub = 0;
                if (parameter != null)
                {
                    double.TryParse(parameter.ToString(), out sub);
                }
                double result = Math.Max(50, total - sub); // ensure minimum width
                return result;
            }
            return 200; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
