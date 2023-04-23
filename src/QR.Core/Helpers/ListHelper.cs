using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers;


//TODO 里面一部分函数需要重写
public static class ListHelper<T> where T : class
{
    /// <summary>
    /// 切割集合数据
    /// </summary>
    /// <param name="source">切割对象</param>
    /// <param name="count">每次切割个数</param>
    /// <param name="group">切割组数</param>
    /// <returns></returns>
    public static List<T> Split(List<T>? source, int count, int group = 1)
        => Range(source, count, group, group);

    /// <summary>
    /// 切割选择范围数据
    /// </summary>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static List<T> Range(List<T>? source, int count, int start, int end)
    {
        if (source == null)
        {
            throw new ArgumentException("输入数据源为空");
        }
        int maxGroup = (int)Math.Ceiling((double)source.Count / (double)count);

        if (end < start)
        {
            throw new ArgumentException("Start cannot greater than end.");
        }
        if (start < 0)
        {
            throw new ArgumentException("Start must greater than 0");
        }
        else if (start == 0)
        {
            return source;
        }
        else if (end > maxGroup)
        {
            throw new ArgumentException("End must less than " + maxGroup.ToString());
        }
        else
        {
            List<T> result = new();

            int totalCount = source.Count;

            int actualStart = count * (start - 1);
            int actualEnd = end * count < totalCount ? end * count : totalCount;

            for (int i = actualStart; i < actualEnd; i++)
            {
                result.Add(source[i]);
            }
            return result;
        }
    }

    /// <summary>
    /// 去重有穷列表
    /// 后插入优先保存
    /// </summary>
    /// <param name="source">数据集合</param>
    /// <param name="length">队列长度</param>
    /// <returns></returns>
    public static List<T>? UniqueRange(List<T> source, int length)
    {
        if (source == null)
        {
            return null;
        }
        List<T> temp = new();

        // 去重
        for (int i = 0; i < source.Count; i++)
        {
            T last = source[i];
            bool isEqual = false;
            for (int j = source.Count - 1; j > i; j--)
            {
                if (last.Equals(source[j]))
                {
                    isEqual = true;
                    break;
                }
            }

            if (!isEqual)
            {
                temp.Add(last);
            }
        }

        // 切割
        if (temp.Count > length)
        {
            temp.RemoveRange(0, temp.Count - length);
        }

        return temp;
    }

    /// <summary>
    /// 向list单个元素之前注入repeat个指定值
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="repeat"></param>
    /// <returns></returns>
    public static List<T> Join(List<T> source, T value, int repeat = 0)
    {
        List<T> temp = new();

        for (int i = 0; i < source.Count; i++)
        {
            for (int j = 0; j < repeat; j++)
            {
                temp.Add(value);
            }
            temp.Add(source[i]);
        }
        return temp;
    }

    /// <summary>
    /// 序列化列表内容
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="serialize"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static void Serialize(List<T> collection, out string? serialize)
    {
        serialize = null;
        if (collection == null) throw new Exception("输入数据源为空");

        try
        {
            serialize = Newtonsoft.Json.JsonConvert.SerializeObject(collection);
        }
        catch (Exception e)
        {
            throw new Exception("序列化失败", e);
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="serialize"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static void Deserialize(string serialize, out List<T> collection)
    {
        collection = new();
        if (string.IsNullOrEmpty(serialize)) new ArgumentException("输入数据源为空");
        try
        {
            var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(serialize);

            if (temp == null) throw new Exception("Deserialize error.The collection is null.");
            else collection = temp;

        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    /// <summary>
    /// 随机内容
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static void Random(List<T> collection)
    {
        Random random = new Random();

        int last = collection.Count - 1;
        int count = collection.Count;

        for (int i = 0; i < count; i++)
        {
            int num = count - i;
            int index = random.Next(0, num);

            T temp = collection[last];
            collection[last] = collection[index];
            collection[index] = temp;

            last--;
        }
    }
}
