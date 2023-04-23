using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace QR.ViewModels;


public class CellViewModel : ObservableObject
{
    public readonly static string CellViewToken = "CellView";

    DispatcherTimer timer;

    /// <summary>
    /// 单词切换之后还原需要的间隔
    /// <para>单位秒</para>
    /// </summary>
    public static double SwitchInterval = 2;

    /// <summary>
    /// 显示什么
    /// </summary>
    private static int ShowMode = 0;

    /// <summary>
    /// 音源
    /// </summary>
    private static int VoiceMode = 0;


    public CellViewModel(MetaWord word)
    {
        Meta = word;

        timer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromSeconds(SwitchInterval)
        };

        timer.Tick += (s, e) =>
        {
            ShowFront();
            timer.Stop();
        };

        WeakReferenceMessenger.Default.Register<string, string>(this, CellViewToken, CellViewModelManage);
    }

    private void CellViewModelManage(object recipient, string message)
    {
        // 解析ShowMode改变的情况
        if (message.Contains("ShowMode"))
        {
            var result = message.Split(new char[] { ' ' });
            if (int.TryParse(result[1], out int value)) ShowMode = value;
        }else if (message.Contains("VoiceMode"))
        {
            var result = message.Split(new char[] { ' ' });
            if (int.TryParse(result[1], out int value)) VoiceMode = value;
        }
        else
        {

        }
    }

    /// <summary>
    /// 数据对象
    /// </summary>
    public MetaWord Meta { get; set; } = new("");

    private string _show = "";

    /// <summary>
    /// 当前界面渲染的内容
    /// </summary>
    public string Show
    {
        get => _show;
        set => SetProperty(ref _show, value);
    }

    /// <summary>
    /// 显示正面信息
    /// </summary>
    public virtual void ShowFront()
    {
        switch (ShowMode)
        {
            case 0:
                Show = "";
                break;
            case 1:
                Show = Meta.ToString();
                break;
            case 2:
                Show = Meta.Word;
                break;
            case 3:
                Show = Meta.InterpretionsStringCompact;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示反面信息
    /// </summary>
    public virtual void ShowBack()
    {
        switch (ShowMode)
        {
            case 0:
                Show = Meta.ToString();
                break;
            case 1:
                Show = Meta.ToString();
                break;
            case 2:
                Show = Meta.InterpretionsStringCompact;
                break;
            case 3:
                Show = Meta.Word;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 朗读单词
    /// </summary>
    public virtual void Speak()
    {
        if (Meta.Voices == null) return;

        Core.Helpers.Player.PlayAsync(Meta.Voices[Meta.Voices.Keys.ToArray()[VoiceMode]]);
    }

    /// <summary>
    /// 恢复初始状态
    /// </summary>
    public void Refresh()
    {
        timer?.Stop();
        ShowFront();
    }

    /// <summary>
    /// 翻个面瞅一下
    /// </summary>
    public void Glance()
    {
        if (!timer.IsEnabled)
        {
            ShowBack();
            timer.Start();
        }
        else // 定时器已经启动了
        {
            ShowFront();
            timer.Stop();
        }
    }

    public override string ToString() => Meta.ToString();
}