using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SecureChatAppTestUI.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public string VisibleValue { get; set; }

        public string HiddenValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return null;
            }
            var bVal = (bool)value;
            if(bVal)
            {
                return VisibleValue;
            }
            else
            {
                return HiddenValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
