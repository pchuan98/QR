using QR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
namespace QR.Core.Helpers.Translator;

public abstract class BaseAPI
{
    public BaseAPI(string word)
    {
        this.InputWord = word;
    }

    protected virtual string HtmlDecode(string value)
        => System.Web.HttpUtility.HtmlDecode(value);

    protected string InputWord { get; set; } = ValueBox.EmptyString;

    /// <summary>
    /// 字典查询网站域名
    /// </summary>
    protected string? URL { get; set; } = ValueBox.EmptyString;

    /// <summary>
    /// 下载是否完成标记位
    /// </summary>
    public bool IsDownload { get; set; } = false;

    /// <summary>
    /// 下载的原始HTML文件
    /// </summary>
    protected string HTML { get; set; } = ValueBox.EmptyString;

    /// <summary>
    /// 初步解析完成的HTML文本对象
    /// </summary>
    protected HtmlDocument Document { get; set; } = new();

    /// <summary>
    /// 之后parse系列方法中要用的东西
    /// </summary>
    protected HtmlNode? ParseContent { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    protected string Word { get; set; } = ValueBox.EmptyString;
    protected Dictionary<string, byte[]?>? Voices { get; set; } = null;
    protected Dictionary<string, string>? Interpretions { get; set; } = null;

    /// <summary>
    /// 下载网页文本
    /// </summary>
    protected abstract void Download();

    /// <summary>
    /// 解析单词的原本形态
    /// </summary>
    protected abstract void ParseHead();

    /// <summary>
    /// 解析单词的读音和音标
    /// </summary>
    protected abstract void ParseVoices();

    /// <summary>
    /// 解析单词的释义集合
    /// </summary>
    protected abstract void ParseInterpretions();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract MetaWord Load();

    /// <summary>
    /// 加载MetaWord数据体
    /// </summary>
    /// <param name="word"></param>
    /// <param name="cover">是否使word中的单词覆盖</param>
    public abstract void Load(ref MetaWord word, bool cover = true);

    /// <summary>
    /// 执行流程
    /// </summary>
    protected virtual void Run()
    {
        this.Download();
        this.ParseHead();
        this.ParseVoices();
        this.ParseInterpretions();
    }
}