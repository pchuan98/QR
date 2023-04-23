using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QR.WPF.Converters;

public class Int2BoolConverter : BaseValueConvert<Int2BoolConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return false;

        if (value is int num && int.TryParse((string)parameter, out var para) && num == para) return true;
        return false;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value != null && value.Equals(true) ? parameter : Binding.DoNothing;
}