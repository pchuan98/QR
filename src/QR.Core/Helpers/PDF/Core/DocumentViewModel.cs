using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace QR.Core.Helpers.PDF;

public class DocumentViewModel : IDocument
{
    /// <summary>
    /// 生成PDF所用的table集合
    /// 按照“流”的形式一个个添加到PDF元素中
    /// </summary>
    public List<TableModel> Tables { get; set; } = new();

    /// <summary>
    /// 页面布局模型
    /// </summary>
    public GridModel Grid { get; set; } = new();

    /// <summary>
    /// 页面参数模型
    /// </summary>
    public PageModel Page { get; set; } = new();

    /// <summary>
    /// 页面生成的元信息
    /// </summary>
    public DocumentMetadata Metadata { get; set; } = DocumentMetadata.Default;

    /// <summary>
    ///
    /// </summary>
    /// <param name="page"></param>
    /// <param name="grid"></param>
    /// <param name="models"></param>
    /// <param name="metadata"></param>
    public DocumentViewModel(PageModel page, GridModel grid, List<TableModel> models, DocumentMetadata? metadata = null)
    {
        Grid = grid;
        Page = page;
        Tables = models;
        Metadata ??= metadata ?? new DocumentMetadata();
    }

    /// <summary>
    /// 简单的验证页面生成参数是否合法
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Valid()
    {
        if (Page == null)
        {
            throw new Exception("请填充PageModel的数据");
        }

        if (Grid == null)
        {
            throw new Exception("请填充GridModel的数据");
        }

        if (Tables == null || Tables.Count == 0)
        {
            throw new Exception("请填充TablesModels的数据");
        }
    }

    /// <summary>
    /// 生成PDF文档元信息
    /// </summary>
    /// <returns></returns>
    public DocumentMetadata GetMetadata() => Metadata;

    /// <summary>
    ///  接口必须实现的函数，用来生成整个Document元素
    /// </summary>
    /// <param name="container"></param>
    public void Compose(IDocumentContainer container)
    {
        this.Valid();

        container.Page(page =>
        {
            page.Size(new PageSize(Page.Width, Page.Height));

            page.MarginLeft((float)Page.Margin[0]);
            page.MarginRight((float)Page.Margin[2]);
            page.MarginTop((float)Page.Margin[1]);
            page.MarginBottom((float)Page.Margin[3]);
            page.Content().Element(ComposeContent);
        });
    }

    /// <summary>
    /// 最核心的方法，用来生成文档的内容
    /// </summary>
    /// <param name="container"></param>
    private void ComposeContent(IContainer container)
    {
        container.Grid(grid =>
        {
            grid.Columns(Grid.ColumnDefinitions);

            grid.HorizontalSpacing(Grid.HorizontalSpacing);
            grid.VerticalSpacing(Grid.VerticalSpacing);

            grid.AlignCenter();

            int pageGridCount = Grid.ColumnDefinitions * Grid.RowDefinitions;

            var size = CalculateGridSize();

            for (int tableIndex = 0; tableIndex < Tables.Count; tableIndex++)
            {
                TableModel tableModel = Tables[tableIndex];


                float[] tableHeights = tableModel.CalculateTableHeights(tableModel.ColumnDefinitions, tableModel.RowDefinitions, size.Width, size.Height);
                float[] tableWidths = tableModel.CalculateTableWidths(tableModel.ColumnDefinitions, tableModel.RowDefinitions, size.Width, size.Height);

                // 给单文件开个小灶
                // 如果是单文件，执行下面的流程，文档不再居中，另外增加了padding的设置
                if (tableModel.RowDefinitions == 1 && tableModel.ColumnDefinitions == 1)
                {
                    grid.Item().Width(size.Width).Height(size.Height).Border(Grid.Border)
                    .ScaleToFit()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(column => column.RelativeColumn(1));
                        table.Cell().ScaleToFit().Padding(tableModel.Border).AlignLeft().AlignTop()
                        .Text(text =>
                        {
                            if (tableModel.Data != null)
                            {
                                foreach (var item in tableModel.Data[0].Split('\n'))
                                {
                                    text.ParagraphSpacing(10);
                                    text.Line(item).Medium();
                                }
                            }
                            text.DefaultTextStyle(
                                TextStyle.Default
                                .FontFamily(tableModel.TableDefaultFontFamily)
                                .FontSize(tableModel.TableDefaultFontSize)
                                );
                        });
                    });

                    return;
                }

                // 正常流程会到这里来
                // 这里一个每个grid的样式会按照给的不同tableModel的参数自适应调整
                grid.Item().Width(size.Width).Height(size.Height).Border(Grid.Border).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < tableModel.ColumnDefinitions; i++)
                        {
                            columns.ConstantColumn(tableWidths[i]);
                        }
                    });

                    if (tableModel.Data != null)
                    {
                        for (int row = 0; row < tableModel.RowDefinitions; row++)
                        {
                            for (int column = 0; column < tableModel.ColumnDefinitions; column++)
                            {
                                int index = row * tableModel.ColumnDefinitions + column;
                                if (index < tableModel.Data.Length)
                                {
                                    table.Cell().Row((uint)row + 1).Column((uint)column + 1)
                                       .MinHeight(tableHeights[row]).MaxHeight(tableHeights[row])
                                       .MinWidth(tableWidths[column]).MaxWidth(tableWidths[column])
                                       .Border(tableModel.Border).ScaleToFit()
                                       .AlignMiddle().AlignCenter()
                                       .Text(tableModel.Data[index])
                                       .FontFamily(tableModel.TableDefaultFontFamily)
                                       .FontSize(tableModel.TableDefaultFontSize);
                                }

                            }
                        }
                    }
                });
            }
        });
    }

    /// <summary>
    /// 通过Page的model和Grid的model计算出每个相等Grid的尺寸
    /// </summary>
    /// <returns></returns> 
    private PageSize CalculateGridSize()
    {
        float height = (float)((Page.Height - Page.Margin[1] - Page.Margin[3] -
           Grid.VerticalSpacing * (Grid.RowDefinitions - 1)) / (float)Grid.RowDefinitions);

        float width = (float)((Page.Width - Page.Margin[0] - Page.Margin[2] -
            Grid.HorizontalSpacing * (Grid.ColumnDefinitions - 1)) / (float)Grid.ColumnDefinitions);

        return new PageSize(width, height);
    }
}