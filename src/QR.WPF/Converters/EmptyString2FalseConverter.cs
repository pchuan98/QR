using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.WPF.Converters;


class EmptyString2FalseConverter : BaseValueConvert<EmptyString2FalseConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.GetType() == typeof(string) && !string.IsNullOrEmpty((string)value);
}