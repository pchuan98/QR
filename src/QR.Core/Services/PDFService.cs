using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR.Core.Helpers.PDF;

namespace QR.Core.Services;

/// <summary>
/// QuickRemember中使用的PDF生成工具集
/// </summary>
public static class PDFHelper
{
    /// <summary>
    /// 文档对象
    /// </summary>
    public static BaseDocumentView? Document = null;

    /// <summary>   
    /// 保存PDF
    /// </summary>
    /// <param name="path">保存地址</param>
    /// <exception cref="Exception"></exception>
    /// <returns>是否保存成功</returns>
    public static bool Save(string path)
    {
        try
        {
            Document?.Save(path);
        }
        catch (IndexOutOfRangeException e)
        {
            throw new Exception("表格尺寸过小，请减小字体或行列数", e);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
        return true;
    }

    #region quarter
    /// <summary>
    /// 四个Frame样式的PDF生成
    /// </summary>
    /// <param name="pageMargin">页面的边距</param>
    /// <param name="hspacing">frame的水平间距</param>
    /// <param name="vspacing">frame的垂直间距</param>
    /// <param name="fontsize">字体大小</param>
    /// <param name="box">打钩的空格数据</param>
    /// <param name="rows">单词行数</param>
    /// <param name="columns">单词列数</param>
    /// <param name="isLandspace">是否横向</param>
    /// <returns>建立文档对象是否成功</returns>
    public static bool GenerateQuarterPDF(
        bool isLandspace,
        float pageMargin,

        float hspacing,
        float vspacing,

        int fontsize,

        List<string> data,

        int rows,
        int columns,

        int box,

        List<bool> boxCollection
        )

    {
        try
        {
            Document = new PureSpaceView()
            {
                IsLandscape = isLandspace,
                PageMargin = new float[4] { pageMargin, pageMargin, pageMargin, pageMargin },

                GridVerticalSpacing = vspacing,
                GridHorizontalSpacing = hspacing,

                SourceData = data,

                TableRows = rows,
                TableColumns = columns,

                SpaceCount = box,
                IsSpaceCollection = boxCollection
            };
            Document.FontSize = fontsize;
            Document.Render();

        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);

        }
        return true;
    }
    #endregion
}