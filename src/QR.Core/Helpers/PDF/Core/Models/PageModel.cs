using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

public class PageModel
{
    public PageSize Size { get; set; } = PageSizes.A4;

    /// <summary>
    /// 是否横向
    /// </summary>
    public bool IsLandscape { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public float Height { get => IsLandscape ? Size.Width : Size.Height; }

    /// <summary>
    /// 
    /// </summary>
    public float Width { get => IsLandscape ? Size.Height : Size.Width; }

    /// <summary>
    /// left,top,right,bottom
    /// </summary>
    public float[] Margin { get; set; } = new float[4] { 0f, 0f, 0f, 0f };
}