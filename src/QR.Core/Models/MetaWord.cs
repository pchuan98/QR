using System;
using System.Collections.Generic;

namespace QR.Core.Models;


/// <summary>
/// 核心单词模型
/// </summary>
[Serializable]
public class MetaWord
{
    /// <summary>
    /// 单词
    /// </summary>
    public string Word { get; set; } = "";

    /// <summary>
    /// 词性-词义
    /// </summary>
    public Dictionary<string, string>? Interpretions { get; set; } = null;

    /// <summary>
    /// 音标-语音
    /// </summary>
    public Dictionary<string, byte[]?>? Voices { get; set; } = null;

    /// <summary>
    /// 判断是否是完整版本
    /// </summary>
    public bool IsFull
    {
        get => !String.IsNullOrEmpty(Word) && Voices != null && Interpretions != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="word"></param>
    /// <param name="interpretion"></param>
    public MetaWord(string word)
    {
        this.Word = word;
    }

    /// <summary>
    /// 释义
    /// </summary>
    public string InterpretionsString
    {
        get
        {
            if (Interpretions == null) return ValueBox.EmptyString;
            else
            {
                try
                {
                    List<string> temp = new();
                    foreach (var key in Interpretions.Keys) temp.Add(String.Format("[{0}] {1}", key, Interpretions[key]));
                    return String.Join("\n", temp);
                }
                catch (Exception)
                {
                    return ValueBox.EmptyString;
                }
            }
        }
    }

    public string InterpretionsStringCompact
    {
        get
        {
            if (Interpretions == null) return ValueBox.EmptyString;
            else
            {
                List<string> temp = new();
                foreach (var key in Interpretions.Keys) temp.Add(String.Format("[{0}] {1}", key, Interpretions[key]));
                return String.Join(" ", temp);
            }
        }
    }

    public string InterpretionsStringFinally
    {
        get
        {
            if (Interpretions == null) return ValueBox.EmptyString;
            else
            {
                List<string> temp = new();
                foreach (var key in Interpretions.Keys) temp.Add(string.Format("{0}", String.Format(Interpretions[key])));
                return String.Join(" | ", temp);
            }
        }
    }

    /// <summary>
    /// 语音
    /// </summary>
    public string VoicesString
    {
        get
        {
            if (Voices == null) return ValueBox.EmptyString;

            else return String.Join(" ", Voices.Keys);
        }
    }

    /// <summary>
    /// 单词加音标的显示模式
    /// </summary>
    public string WordGroup
    {
        get
        {
            if (Voices == null) return Word;

            else return String.Join("\n", Word, String.Join(" ", Voices.Keys));
        }
    }

    public string CSVString
    {
        get => String.Format("{0},{1},{2}", Word, VoicesString, InterpretionsStringCompact);
    }

    public override string ToString()
        => String.Join("\n", Word, InterpretionsStringCompact);

    public string ToString(bool isSplit)
        => String.Join("\n", Word, VoicesString,InterpretionsStringCompact);
}