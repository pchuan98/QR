using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QR.WPF.Converters;

public class ReverseBoolConverter : BaseValueConvert<ReverseBoolConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value == null ? false : !((bool)value);

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value == null ? false : !((bool)value);
}