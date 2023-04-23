using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QR.Core;
using QR.Core.Helpers;
using QR.Core.Models;
using QR.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QR.ViewModels;

public class PrintViewModel : ObservableObject
{
    GridViewModel GVM;
    // 抽象结构  之后可能会改
    GridViewModel GridVM;

    public AsyncRelayCommand GeneratePDFCommand { get; set; }

    public RelayCommand OpenPDFCommand { get; set; }

    public PrintViewModel(GridViewModel vm)
    {
        // 这个是用来返回数据的
        GVM = vm;

        // 这是实际使用的
        GridVM = vm;

        GeneratePDFCommand = new(GeneratePDF);

        OpenPDFCommand = new(() =>
        {
            try
            {
                System.Diagnostics.Process.Start("Explorer.exe", PDFPath);
            }
            catch (Exception e)
            {
                AddMessage(e.Message);
            }
        });
    }

    private double _page = 15.0;
    public double PageMargin
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }

    private double _vertical = 30.0;
    public double VerticalSpace
    {
        get => _vertical;
        set => SetProperty(ref _vertical, value);
    }

    private double _horizontal = 30.0;
    public double HorizontalSpace
    {
        get => _horizontal;
        set => SetProperty(ref _horizontal, value);
    }

    private int _box = 2;
    public int BoxCount
    {
        get => _box;
        set => SetProperty(ref _box, value);
    }

    private bool _allen = false;
    public bool IsDoubleEnglish
    {
        get => _allen;
        set => SetProperty(ref _allen, value);
    }

    private bool _israndom = false;
    public bool IsRandom
    {
        get => _israndom;
        set => SetProperty(ref _israndom, value);
    }

    private bool _islandscape = true;
    public bool IsLandScape
    {
        get => _islandscape;
        set => SetProperty(ref _islandscape, value);
    }

    private bool _isImage = false;
    public bool IsImage
    {
        get => _isImage;
        set => SetProperty(ref _isImage, value);
    }

    private string _msg = "";
    public string Message
    {
        get => _msg;
        set
        {
            // TODO: 当value行数大于100 自动清理


            SetProperty(ref _msg, value);
        }
    }

    private int _fontsize = 10;
    public int FontSize
    {
        get => _fontsize;
        set => SetProperty(ref _fontsize, value);
    }

    private bool _isfillay = false;
    public bool IsFinally
    {
        get => _isfillay;
        set
        {
            SetProperty(ref _isfillay, value);

            if (value)
            {
                IsDoubleEnglish = false;
                IsLandScape = false;
                IsImage = false;
                BoxCount = 0;

                VerticalSpace = 20;
                HorizontalSpace = 20;
                PageMargin = 15;
            }
        }
    }


    #region 数据抽象
    List<MetaWord> WordCollection { get => GridVM.WordCollection; }
    List<CellViewModel> ShowCollection { get => GridVM.ShowCollection; }
    int Columns { get => GridVM.Columns; }
    int Rows { get => GridVM.Rows; }
    #endregion

    private string _pdfpath = "";
    public string PDFPath
    {
        get => _pdfpath;
        set => SetProperty(ref _pdfpath, value);
    }


    private void AddMessage(string msg, bool time = true)
        => Message += time ? (DateTime.Now.ToString("F") + " : " + msg + "\n") : msg + "\n";

    /// <summary>
    /// 生成 PDF
    /// TODO：当前支持的PDF格式还比较少，之后得添加更多的格式，并且开放一些其他的操作格式
    /// HACK：这里的生成代码比较复杂，之后得优化一下，并且抽象更多的model方便解构
    /// TODO:这里生成的成功率和内存大小挂钩
    /// </summary>
    /// <returns></returns>
    private async Task GeneratePDF()
    {
        if (WordCollection == null) return;

        if (ShowCollection == null || ShowCollection.Count == 0)
        {
            AddMessage("数据为空");
            return;
        }

        PDFPath = "";

        if (!IsFinally)
        {
            await Task.Factory.StartNew(() =>
            {
                if (!IsImage && !(Rows % 2 == 0 || Columns % 2 == 0))
                {
                    AddMessage("非镜像模式要求列数或行数为偶数");
                    return;
                }

                if (Columns == 1 || Rows == 1) AddMessage("不支持太小的行列生成PDF");

                AddMessage("PDF数据准备中......");

                int column = IsImage ? Columns : Columns / 2;
                int row = Rows;
                int box = BoxCount;

                bool landscape = IsLandScape;
                float margin = (float)PageMargin;
                float vspacing = (float)VerticalSpace;
                float hspacing = (float)HorizontalSpace;

                List<bool> spaces = new();

                List<CellViewModel> src = new();
                ShowCollection.ForEach(item => src.Add(item));

                List<string> dst = new();

                // 两个数据组和
                List<CellViewModel> part1 = new();
                List<CellViewModel> part2 = new();

                List<string> temp = new();

                try
                {

                    // 镜像模式把一份数据打印到左右两个page上
                    if (IsImage)
                    {
                        part1 = src;
                        part2 = src;
                    }
                    else
                    {
                        // 对半分
                        int half = (int)(src.Count / 2.0);

                        // 第一部分
                        part1 = ListHelper<CellViewModel>.Split(src, half, 1);
                        part2 = ListHelper<CellViewModel>.Split(src, half, 2);
                    }

                    if (!IsDoubleEnglish)
                    {
                        spaces = new() { true, true, false, false };

                        // 随机结果
                        if (IsRandom)
                        {
                            ListHelper<CellViewModel>.Random(part1);
                            ListHelper<CellViewModel>.Random(part2);
                        }

                        // 填充四个table的数据
                        temp = new();
                        part1.ForEach(item => temp.Add(item.Meta.Word));
                        RangeList(temp, column * row).ForEach(item => dst.Add(item));

                        temp = new();
                        part2.ForEach(item => temp.Add(item.Meta.Word));
                        RangeList(temp, column * row).ForEach(item => dst.Add(item));

                        temp = new();
                        part1.ForEach(item => temp.Add(item.Meta.InterpretionsStringCompact));
                        RangeList(temp, column * row).ForEach(item => dst.Add(item));

                        temp = new();
                        part2.ForEach(item => temp.Add(item.Meta.InterpretionsStringCompact));
                        RangeList(temp, column * row).ForEach(item => dst.Add(item));
                    }
                    else
                    {
                        spaces = new() { true, true, true, true };

                        // 填充两次
                        for (int i = 0; i < 2; i++)
                        {
                            if (IsRandom)
                            {
                                ListHelper<CellViewModel>.Random(part1);
                                ListHelper<CellViewModel>.Random(part2);
                            }

                            // 填充四个table的数据
                            temp = new();
                            part1.ForEach(item => temp.Add(item.Meta.Word));
                            RangeList(temp, column * row).ForEach(item => dst.Add(item));

                            temp = new();
                            part2.ForEach(item => temp.Add(item.Meta.Word));
                            RangeList(temp, column * row).ForEach(item => dst.Add(item));

                        }
                    }

                    // 对dst数据规范


                }
                catch (Exception e)
                {
                    AddMessage(e.Message);
                    return;
                }

                AddMessage("PDF数据准备完成");

                try
                {
                    PDFHelper.GenerateQuarterPDF(
                        landscape, margin,
                        hspacing, vspacing,
                        FontSize,
                        dst, row,
                        column, box, spaces);

                    if (FileService.ShowFileDialog(out var path, "pdf file(*.pdf)|*.pdf|所有文件(*.*)|*.*", "保存PDF文件", DlgType.SaveDlg))
                    {
                        PDFHelper.Save(path);
                        AddMessage("PDF保存在：" + path);
                        PDFPath = path;

                        OtherInfo();
                    }
                    else AddMessage("取消打印");

                }
                catch (Exception)
                {
                    AddMessage("渲染失败");
                    AddMessage("PDF渲染库太垃圾了，可能原因很多");
                    AddMessage("包括但不限于：当前可用内存太小，中文内容太长，参数有误...");
                    AddMessage("请降低Columns或Rows，或者改成单词+单词模式");
                    return;
                }


            });
        }

        if (IsFinally)
        {


            await Task.Factory.StartNew(() =>
            {
                // 收集原始数据
                List<CellViewModel> src = new();
                ShowCollection.ForEach(item => src.Add(item));

                // 随机判断
                if (IsRandom) ListHelper<CellViewModel>.Random(src);

                // 构建需要的数据结构
                List<string> data = new();
                src.ForEach(item =>
                {
                    data.Add(item.Meta.Word);
                    data.Add(item.Meta.InterpretionsStringFinally);
                });

                // 单词的总数
                int count = ShowCollection.Count;

                // panel尺寸
                int row = (int)Math.Floor(count / 4.0);
                int column = 2;

                float margin = (float)PageMargin;
                float vspacing = (float)VerticalSpace;
                float hspacing = (float)HorizontalSpace;

                // 填补空白部分
                if (row * column > count)
                {
                    int empty = row * 2 * 4 - count;
                    for (int i = 0; i < empty; i++)
                    {
                        data.Add("");
                        data.Add("");
                    }
                }

                try
                {
                    PDFHelper.GenerateQuarterPDF(
                        false, 
                        margin, 
                        hspacing, 
                        vspacing, 
                        FontSize, 
                        data, 
                        row, 
                        column, 
                        0, 
                        new() { false, false, false, false }
                        );

                    if (FileService.ShowFileDialog(out var path, "pdf file(*.pdf)|*.pdf|所有文件(*.*)|*.*", "保存PDF文件", DlgType.SaveDlg))
                    {
                        PDFHelper.Save(path);
                        AddMessage("PDF保存在：" + path);
                        PDFPath = path;

                        OtherInfo();
                    }
                    else AddMessage("取消打印");

                }
                catch (Exception)
                {
                    AddMessage("渲染失败");
                    AddMessage("PDF渲染库太垃圾了，可能原因很多");
                    AddMessage("包括但不限于：当前可用内存太小，中文内容太长，参数有误...");
                    AddMessage("请降低Columns或Rows，或者改成单词+单词模式");
                    return;
                }
            });
        }
    }


    private void OtherInfo()
    {
        AddMessage("=========================================", false);
        AddMessage("打印完成", false);
        AddMessage(PDFPath);
        if (IsLandScape == false) AddMessage("当前未设置横向，列数过多时可能会排版不正确", false);
        if (BoxCount > 2) AddMessage("当前方格数过多，可能会出现排版错误", false);

        AddMessage("=========================================", false);

    }

    private List<string> RangeList(List<string> collection, int count)
    {
        var temp = new List<string>();
        var cCount = collection.Count;

        collection.ForEach(item => temp.Add(item));

        if (count < cCount) return ListHelper<string>.Split(temp, count, 1);
        else
        {
            for (int i = 0; i < count - cCount; i++)
            {
                temp.Add("");
            }
            return temp;
        }
    }
}