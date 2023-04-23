using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QR.WPF.Converters;


public class Interpretion2String : BaseValueConvert<Interpretion2String>
{
    private static readonly Regex InterpretionRegex = new Regex(@"\[.*?\]");

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Dictionary<string, string> interpretion)
        {
            List<string> temp = new();
            foreach (var key in interpretion.Keys) temp.Add(String.Format("[{0}] {1}", key, interpretion[key]));
            return String.Join("\n", temp);
        }
        return String.Empty;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var aaa = parameter;
        if (value is string merge)
        {
            try
            {
                var lines = merge.Split(new char[] { '\n' });
                var dict = new Dictionary<string, string>();


                foreach (var line in lines)
                {
                    var temp = InterpretionRegex.Match(line.Trim());
                    if (temp == null || !temp.Success) continue;

                    dict[line.Substring(temp.Index + 1, temp.Length - 2).Trim()] = line.Substring(temp.Index + temp.Length).Trim();
                }
                return dict;

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        return new Dictionary<string, string>();
    }
}