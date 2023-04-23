using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QR.WPF.Converters;

public class Item2IntConverter : BaseValueConvert<Item2IntConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComboBoxItem item && int.TryParse((string)item.Tag, out var result)) return result;
        return 0;
    }
}