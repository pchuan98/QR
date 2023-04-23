using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QR.WPF;


public static class ThemeHelper
{

    private readonly static string ThemeURL = "pack://application:,,,/HandyControl;component/Themes/Theme.xaml";

    private readonly static string LightURL = "pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml";

    private readonly static string DarkURL = "pack://application:,,,/HandyControl;component/Themes/SkinDark.xaml";

    private readonly static string VioletURL = "pack://application:,,,/HandyControl;component/Themes/SkinViolet.xaml";

    private readonly static List<string> SkinURLs = new() { LightURL, DarkURL, VioletURL };

    public static void UpdateGlobalSkin(byte mode)
    {
        var dirs = Application.Current.Resources.MergedDictionaries;

        // 判断全局有没有几个Skin URL
        if (IsMergeSkin(out var i)) dirs.RemoveAt(i);
        dirs.Add(new() { Source = new(SkinURLs[mode]) });

        if (IsMerge(ThemeURL, out var j)) dirs.RemoveAt(j);
        dirs.Add(new() { Source = new(ThemeURL) });
    }



    /// <summary>
    /// 判断是否合并过Skin了
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private static bool IsMergeSkin(out int count)
    {
        count = -1;
        var dicts = Application.Current.Resources.MergedDictionaries;

        for (int i = 0; i < dicts.Count; i++)
        {
            if (SkinURLs.Exists(t => t == dicts[i].Source.ToString()))
            {
                count = i;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断是否已经合并过了
    /// </summary>
    /// <param name="url"></param>
    /// <param name="count"></param>
    /// <returns></returns>

    private static bool IsMerge(string url, out int count)
    {
        count = -1;
        var dicts = Application.Current.Resources.MergedDictionaries;

        for (int i = 0; i < dicts.Count; i++)
        {
            if (dicts[i].Source.ToString() == url)
            {
                count = i;
                return true;
            }
        }
        return false;
    }
}