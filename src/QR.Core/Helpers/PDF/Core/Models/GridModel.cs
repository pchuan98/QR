using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

/// <summary>
/// 每个Document对象只持有一个GridModel
/// </summary>
public class GridModel
{
    /// <summary>
    /// page下分多少个列
    /// </summary>
    public int ColumnDefinitions { get; set; } = 2;

    /// <summary>
    /// page下分多少个行
    /// </summary>
    public int RowDefinitions { get; set; } = 2;

    /// <summary>
    /// 不同grid垂直方向的间距
    /// </summary>
    public float VerticalSpacing { get; set; } = 10f;

    /// <summary>
    /// 不同grid水平方向的间距
    /// </summary>
    public float HorizontalSpacing { get; set; } = 10f;

    /// <summary>
    /// 单个Grid的边界尺寸
    /// </summary>
    public float Border { get; set; } = 1f;
}
