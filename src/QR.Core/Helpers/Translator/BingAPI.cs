using HtmlAgilityPack;
using QR.Core.Error;
using QR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QR.Core.Helpers.Translator;

public class BingAPI : BaseAPI
{
    Regex URLRegex;
    Regex PhoneticRegex;
    public BingAPI(string word = "") : base(word)
    {
        this.URL = @"https://cn.bing.com/dict/search";
        this.URLRegex = new Regex("https.*?mp3");
        this.PhoneticRegex = new Regex(@"\[.*?\]");
    }


    protected override void Download()
    {
        try
        {
            var url = String.Format("{0}?q={1}", this.URL, this.InputWord);
            this.HTML = Downloader.DownloadString(url);

            this.Document.LoadHtml(this.HTML);
            this.ParseContent = this.Document.DocumentNode.SelectSingleNode("//*/div[@class='qdef']");
        }
        catch (Exception e)
        {
            throw new Exception("下载解析错误。", e);
        }
    }

    protected override void ParseHead()
    {
        try
        {
            if (this.ParseContent == null) throw new AssertException("待解析实体单词内容为空");

            this.Word = this.ParseContent.SelectSingleNode(@"//div[@id='headword']").InnerText;
            this.Word = this.HtmlDecode(this.Word);
        }
        catch (Exception e)
        {
            throw new Exception("解析单词实体错误", e);
        }
    }

    protected override void ParseInterpretions()
    {
        try
        {
            if (this.ParseContent == null) throw new AssertException("待解析释义内容为空");

            var content = this.ParseContent.SelectSingleNode("ul")?.ChildNodes;

            if (content == null) throw new AssertException("没有解析到单纯释义数据");

            this.Interpretions = new();

            foreach (var child in content)
            {
                var temp = child.ChildNodes;
                if (temp.Count != 2) throw new AssertException("网页释义数据不符合解析规则");
                this.Interpretions[this.HtmlDecode(temp[0].InnerText).Replace(@".", "")] = this.HtmlDecode(temp[1].InnerText);
            }
        }
        catch (AssertException e)
        {
            throw e;
        }
        catch (Exception e)
        {
            throw new Exception("解析单词释义错误", e);

        }
    }

    protected override void ParseVoices()
    {
        try
        {
            if (this.ParseContent == null) throw new AssertException("待解析语音内容为空");

            if (Word.Split(new char[] { ' ' }).Length == 2) return;

            var voices = this.ParseContent.SelectSingleNode(@"//div[@class='hd_p1_1']").ChildNodes;

            if (voices.Count != 4) throw new AssertException("语音内容解析不符合条件");

            var key1 = String.Format("{0} {1}", "美", PhoneticRegex.Match(voices[0].InnerHtml).Value);
            var value1 = voices[1].FirstChild.Attributes["onClick"].Value;
            var url1 = this.URLRegex.Match(value1).Value;


            var key2 = String.Format("{0} {1}", "英", PhoneticRegex.Match(voices[2].InnerText).Value);
            var value2 = voices[3].FirstChild.Attributes["onClick"].Value;
            var url2 = this.URLRegex.Match(value2).Value;

            var voice1 = Downloader.DownloadBytes(url1);
            var voice2 = Downloader.DownloadBytes(url2);

            if (voice1 == null || voice2 == null) throw new AssertException("单词语音下载失败");

            this.Voices = new();

            this.Voices[this.HtmlDecode(key1)] = voice1;
            this.Voices[this.HtmlDecode(key2)] = voice2;

        }
        catch (AssertException e)
        {
            throw e;
        }
        catch (Exception e)
        {
            throw new Exception("解析单词音标语音错误", e);
        }

    }

    public override MetaWord Load()
    {
        MetaWord word = new(InputWord);

        this.Run();

        word.Word = this.Word;
        word.Voices = this.Voices;
        word.Interpretions = this.Interpretions;

        return word;
    }

    public override void Load(ref MetaWord word, bool cover = true)
    {
        this.InputWord = cover ? word.Word : this.InputWord;

        this.Run();

        word.Word = this.Word;
        word.Voices = this.Voices;
        word.Interpretions = this.Interpretions;
    }

    /// <summary>
    /// 一个直接能调用的方法 简化流程
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static MetaWord QuickLoad(string word)
    {
        MetaWord meta = new(word);


        var bing = new BingAPI(word);
        meta = bing.Load();

        return meta;
    }
}