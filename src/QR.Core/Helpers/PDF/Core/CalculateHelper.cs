using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

public static class CalculateHelper
{
    #region Average

    private static float[] AverageSize(int count, float s)
    {
        float[] result = new float[count];
        float size = s / (float)count;
        for (int i = 0; i < count; i++)
        {
            result[i] = size;
        }
        return result;
    }

    /// <summary>
    /// 均分行高度
    /// </summary>
    /// <param name="c"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public static float[] AverageHeight(int c, int r, float w, float h) => AverageSize(r, h);

    /// <summary>
    /// 均分列宽度
    /// </summary>
    /// <param name="c"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public static float[] AverageWidth(int c, int r, float w, float h) => AverageSize(c, w);

    #endregion Average

    #region Box

    /// <summary>
    /// 小方格模式
    /// </summary>
    /// <param name="count">单次循环中小方格的数量</param>
    /// <param name="repeatCount">重复次数,本质是有数据的列数</param>
    /// <param name="cd">列数</param>
    /// <param name="rd">行数</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static float[] BoxSizeWidth(int count, int repeatCount, int cd, int rd, float width, float height)
    {
        // valid
        if ((count + 1) * repeatCount != cd) throw new Exception("小方块的分割参数不符合要求列数");

        float[] result = new float[cd];

        // 总方格+单格宽度
        float cell = width / (float)repeatCount;
        // 方格宽度
        float box = height / (float)rd;
        // 单格宽度
        float remain = cell - box * count;

        if (remain <= 0) throw new Exception("小方块的分割次数过多或重复次数过多");

        for (int i = 0; i < repeatCount; i++)
        {
            for (int j = 0; j < count; j++)
            {
                result[i * (count + 1) + j] = box;
            }
            result[(count + 1) * i + count] = remain;
        }

        return result;
    }

    #endregion Box

    #region Scale

    /// <summary>
    /// 按比例分派尺寸
    /// </summary>
    /// <param name="scales"></param>
    /// <param name="count"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static float[] ScaleSize(int[] scales, int count, float size)
    {
        if (scales.Length != count) throw new Exception("缩放数组长度和分割数不匹配");

        float[] result = new float[count];

        // 求和
        int number = 0;
        foreach (var scale in scales)
        {
            number += scale;
        }

        for (int i = 0; i < scales.Length; i++)
        {
            result[i] = size * ((float)scales[i] / (float)number);
        }
        return result;
    }

    #endregion Scale
}