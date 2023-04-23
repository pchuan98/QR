using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

public abstract class BaseDocumentView
{
    /// <summary>
    /// 生产默认的元信息
    /// </summary>
    /// <param name="author"></param>
    /// <param name="title"></param>
    /// <param name="subject"></param>
    /// <param name="keyword"></param>
    /// <param name="creator"></param>
    /// <param name="producer"></param>
    /// <returns></returns>
    public DocumentMetadata ConfigMetadata(
        string author = "Toolkits",
        string title = "Toolkits Title",
        string subject = "",
        string keyword = "Toolkits",
        string creator = "Toolkits",
        string producer = "PPPCCC")
    {
        DocumentMetadata Metadata;
        Metadata = new DocumentMetadata();
        Metadata.Author = author;
        Metadata.Title = title;
        Metadata.Subject = subject;
        Metadata.Keywords = keyword;
        Metadata.Creator = creator;
        Metadata.Producer = producer;
        return Metadata;
    }

    /* 
     * ---------------------------------------------------
     * Page Model
     * ---------------------------------------------------
     */

    /// <summary>
    /// [width,height]
    /// </summary>
    public float[] Size { get; set; } = new float[2] { PageSizes.A4.Width, PageSizes.A4.Height };

    private PageSize PageSize { get => new PageSize(Size[0], Size[1]); }

    /// <summary>
    /// 
    /// </summary>
    public bool IsLandscape { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public float[] PageMargin { get; set; } = new float[4] { 10f, 10f, 10f, 10f };

    /* 
     * ---------------------------------------------------
     * Grid Model
     * ---------------------------------------------------
     */

    /// <summary>
    /// 
    /// </summary>
    public int GridColumns { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public int GridRows { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public float GridVerticalSpacing { get; set; } = 10f;

    /// <summary>
    /// 
    /// </summary>
    public float GridHorizontalSpacing { get; set; } = 10f;

    /// <summary>
    /// 
    /// </summary>
    public float GridBorder { get; set; } = 1f;

    /* 
     * ---------------------------------------------------
     * Table Model
     * ---------------------------------------------------
     */

    /// <summary>
    /// 原始数据集
    /// </summary>
    public List<string> SourceData { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public int TableRows { get; set; } = 10;

    /// <summary>
    /// 
    /// </summary>
    public int TableColumns { get; set; } = 3;

    /// <summary>
    /// 
    /// </summary>
    public float TableBorder { get; set; } = 0.5f;

    /// <summary>
    /// 
    /// </summary>
    public int FontSize { get; set; } = 8;

    /// <summary>
    /// 
    /// </summary>
    public string FontFamily { get; set; } = "SimSun";

    /// <summary>
    /// 初始化参数
    /// </summary>
    public abstract void OnInitialized();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract List<TableModel> CreateTableModels();

    protected DocumentViewModel? Document { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public virtual bool Render()
    {
        OnInitialized();

        // page model
        PageModel page = new();
        page.Size = this.PageSize;
        page.IsLandscape = this.IsLandscape;
        page.Margin = this.PageMargin;

        // grid
        GridModel grid = new();
        grid.RowDefinitions = this.GridRows;
        grid.ColumnDefinitions = this.GridColumns;
        grid.VerticalSpacing = this.GridVerticalSpacing;
        grid.HorizontalSpacing = this.GridHorizontalSpacing;
        grid.Border = this.GridBorder;

        // tables

        List<TableModel> tables = this.CreateTableModels();
        Document = new DocumentViewModel(page, grid, tables, this.ConfigMetadata());


        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public virtual void Save(string path)
        => Document?.GeneratePdf(path);
}