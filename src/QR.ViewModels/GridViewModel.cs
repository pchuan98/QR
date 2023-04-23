using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QR.ViewModels;

public class GridViewModel : ObservableObject
{
    public readonly static string GridToken = "GRIDVIEW";

    /// <summary>
    /// 原始形态的数据
    /// </summary>
    public List<Core.Models.MetaWord> WordCollection { get; set; } = new();

    /// <summary>
    /// 待用的数据
    /// </summary>
    public List<CellViewModel> CellCollection { get; set; } = new();

    private List<CellViewModel> _showcollectoin = new();

    /// <summary>
    /// 显示数据
    /// </summary>
    public List<CellViewModel> ShowCollection
    {
        get => _showcollectoin;
        set => SetProperty(ref _showcollectoin, value);
    }

    private int _rows = 18;
    public int Rows
    {
        get => _rows;
        set
        {
            SetProperty(ref _rows, value);
            OnRowsChanged();
        }
    }



    private int _columns = 6;
    public int Columns
    {
        get => _columns;
        set
        {
            SetProperty(ref _columns, value);
            OnColumnsChanged();
        }
    }


    private int _page = 1;
    public int Page
    {
        get => _page;
        set
        {
            SetProperty(ref _page, value);
            OnPageChanged();
        }
    }

    private int _maxpage = 1;
    public int MaxPage
    {
        get => _maxpage;
        set => SetProperty(ref _maxpage, value);
    }

    private int _mode = 2;
    public int ShowMode
    {
        get => _mode;
        set
        {
            SetProperty(ref _mode, value);
            OnShowModeChanged();
        }
    }

    private int _voicemode = 1;
    public int VoiceMode
    {
        get => _voicemode;
        set
        {
            SetProperty(ref _voicemode, value);
            OnVoiceModeChanged();
        }
    }

    private int _sortmode = 0;
    public int SortMode
    {
        get => _sortmode;
        set
        {
            SetProperty(ref _sortmode, value);
            OnSortChanged();
        }

    }
    public RelayCommand ReadFileCommand { get; set; }
    public RelayCommand RandomAllCollectionCommand { get; set; }
    public RelayCommand SingleWordCommand { get; set; }

    public GridViewModel(List<Core.Models.MetaWord> words)
    {
        WordCollection = words;

        // 基础数据加载
        Loading();

        ReadFileCommand = new(() => { if (ReadFileWithDialog()) Loading(); });
        RandomAllCollectionCommand = new(() =>
        {
            if (WordCollection == null || WordCollection.Count < 1) return;
            // 随机collection里面的数据
            ListHelper<CellViewModel>.Random(CellCollection);

            // 刷新当前视图
            RefreshShowCollection();
        });

        SingleTimer.Interval = TimeSpan.FromSeconds(3);
        SingleTimer.Tick += (s, e) => {
            if (WordIndex >= CellCollection.Count)
            {
                SingleTimer.Stop();
                return;
            }

            ShowCollection = new() { CellCollection[WordIndex] };
            CellCollection[WordIndex].Speak();

            WordIndex++;
        };

        SingleWordCommand = new(() => {
            // 如果已经使用了，就关闭
            if (SingleTimer.IsEnabled)
            {
                SingleTimer.Stop();
                return;
            }

            // 如果没有，那么开始执行
            Rows = 1;
            Columns = 1;
            ShowMode = 1;
            VoiceMode = 0;

            WordIndex = 0;

            SingleTimer.Start();
        });
    }

    // SingleWordCommand使用的变量
    int WordIndex = 0;
    DispatcherTimer SingleTimer = new();

    /// <summary>
    /// 程序执行开始阶段准备
    /// </summary>
    private void Loading()
    {
        // 循环添加数据
        CellCollection.Clear();
        WordCollection.ForEach(item => CellCollection.Add(new(item)));

        // 通知
        InformShowMode();
        InformVoiceMode();

        // 简单的刷新视图
        RefreshMaxPage();
        RefreshShowCollection();
    }

    /// <summary>
    /// 总线程序
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="message"></param>
    private void GridViewModelManage(object recipient, string message)
    {

    }

    /// <summary>
    /// 行列改变的时候
    /// </summary>
    private void OnSizeChanged()
    {
        RefreshMaxPage();
        if (Page > MaxPage) Page = MaxPage;

        RefreshShowCollection();
    }

    private void OnRowsChanged() => OnSizeChanged();

    private void OnColumnsChanged() => OnSizeChanged();

    private void OnPageChanged() => RefreshShowCollection();

    private void OnShowModeChanged()
    {
        InformShowMode();
        RefreshCells();
    }

    private void OnVoiceModeChanged() => InformVoiceMode();

    /// <summary>
    /// 原始 随机 顺序 逆序
    /// </summary>
    private void OnSortChanged() => RefreshShowCollection();

    #region 纯粹的功能
    /// <summary>
    /// 刷新最大的Page数
    /// </summary>
    private void RefreshMaxPage()
        => MaxPage = (int)Math.Ceiling(WordCollection.Count / (double)(Rows * Columns));

    /// <summary>
    /// 更新当前用的显示数据
    /// </summary>
    private void RefreshShowCollection()
    {
        try
        {
            var items = ListHelper<CellViewModel>.Split(CellCollection, Rows * Columns, Page);

            switch (SortMode)
            {
                case 0:
                    break;
                case 1:
                    ListHelper<CellViewModel>.Random(items);
                    break;
                case 2:
                    items.Sort((x, y) =>
                    {
                        string src = x.Meta.Word.ToLower();
                        string dst = y.Meta.Word.ToLower();
                        return src.CompareTo(dst);
                    });
                    break;
                case 3:
                    items.Sort((x, y) =>
                    {
                        string src = x.Meta.Word.ToLower();
                        string dst = y.Meta.Word.ToLower();
                        return dst.CompareTo(src);
                    });
                    break;
                default:
                    break;
            }

            ShowCollection = items;
        }
        catch (Exception e) { }
    }

    private void InformShowMode()
        => WeakReferenceMessenger.Default.Send<string, string>(String.Format("ShowMode {0}", ShowMode), CellViewModel.CellViewToken);

    private void InformVoiceMode()
        => WeakReferenceMessenger.Default.Send<string, string>(String.Format("VoiceMode {0}", VoiceMode), CellViewModel.CellViewToken);

    /// <summary>
    /// 视图文件的刷新
    /// </summary>
    private void RefreshCells() => ShowCollection.ForEach(item => item.Refresh());

    private bool ReadFileWithDialog()
    {
        try
        {
            if (Core.Services.FileService.ReadFilesDialog(out var path) && path.Count > 0)
            {
                var words = new List<Core.Models.MetaWord>();
                foreach (var item in path)
                {
                    Core.Services.FileService.ReadWords(item, out var collection);
                    words = words.Union(collection).ToList();
                }

                WordCollection = words;
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            System.Windows.MessageBox.Show(e.Message);
        }
        return false;
    }
    #endregion
}