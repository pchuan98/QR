using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QR.WPF.Converters;

public class AutoStringSplitConverter : BaseValueConvert<AutoStringSplitConverter>
{
    private static readonly int Count = 25;
    private static readonly Regex EnglishReg = new Regex(@"^[a-zA-Z\-\s]+$");
    private static readonly Regex DoubleReg = new Regex(@"^([a-zA-Z\-\s]+)\n(.*?)$");
    private static readonly char[] NewLine =new char[] { '\n' };
    private static int MaxLength = 4;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value == null) return "";

            var str = (string)value;

            if (EnglishReg.IsMatch(str)) return value;

            // 最终结果
            List<string> lines = new();

            // 匹配英语加中文的模式  后期这里都得改 开销也太大了
            if (DoubleReg.IsMatch(str))
            {
                // 这里好麻烦  先不写了
                // 早知道还不如一开始就把单词抽象成Card来看 直接写个显示用的控件得了

                var split = str.Split(NewLine);

                lines.Add(split[0]);

                str = String.Join("", split.Skip(1).ToArray());

                MaxLength = 3;
            }

            // 处理剩余字符
            str = str.Replace("\n", "");
            str = str.Replace(" ", "");

            var length = str.Length;
            var lineCount = (int)Math.Ceiling((double)length / (double)Count);
            lineCount = lineCount < MaxLength ? lineCount : MaxLength;
            int actualLength = (int)Math.Floor((double)length / (double)lineCount);

            for (int i = 0; i < lineCount; i++)
            {
                lines.Add(str.Substring(i * actualLength, actualLength));
            }

            // 复原
            MaxLength = 4;

            return string.Join("\n", lines);

        }
        catch (Exception)
        {
            return "";
        }
    }
}