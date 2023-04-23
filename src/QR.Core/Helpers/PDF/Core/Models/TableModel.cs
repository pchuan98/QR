using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

/// <summary>
/// 每个Document都可以拥有多个TableModel
/// TableModel中的Data将按照每个page的grid样式自动分配出去
/// 具体的表格尺寸放到viewmodel中计算，利用委托的方法把接口暴露出去
/// </summary>
public class TableModel
{
    public CalculateTableSize CalculateTableWidths { set; get; } = CalculateHelper.AverageWidth;
    public CalculateTableSize CalculateTableHeights { set; get; } = CalculateHelper.AverageHeight;

    /// <summary>
    /// 数据填充的过程是先行后列
    /// </summary>
    public string[]? Data { get; set; } = null;

    /// <summary>
    /// 数据列数
    /// </summary>
    public int ColumnDefinitions { get; set; } = 3;

    /// <summary>
    /// 数据行数
    /// </summary>
    public int RowDefinitions { get; set; } = 15;

    /// <summary>
    /// 每个单元格的border宽度
    /// </summary>
    public float Border { get; set; } = 0.5f;

    /// <summary>
    /// 默认文字尺寸
    /// </summary>
    public int TableDefaultFontSize { get; set; } = 14;

    /// <summary>
    /// 文字字体
    /// </summary>
    public string TableDefaultFontFamily { get; set; } = "SimSun";
}