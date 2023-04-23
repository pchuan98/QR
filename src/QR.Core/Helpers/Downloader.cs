using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers;

public class Downloader
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly HttpClient Client;

    /// <summary>
    /// 
    /// </summary>
    static Downloader()
    {
        Client = new HttpClient();
    }

    /// <summary>
    /// 异步下载网页文本数据
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<string> DownloadStringAsync(string url)
    {
        string content = "";
        try
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            content = await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
        return content;
    }

    /// <summary>
    /// 下载网页文本
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string DownloadString(string url)
    {
        string content = "";
        try
        {
            var response = Client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            content = response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
        return content;
    }

    /// <summary>
    /// 异步下载二进制数据
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<byte[]?> DownloadBytesAsync(string url)
    {
        byte[]? content = null;
        try
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            content = await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
        return content;
    }

    /// <summary>
    /// 下载二进制数据
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static byte[]? DownloadBytes(string url)
    {
        byte[]? content = null;
        try
        {
            var response = Client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            content = response.Content.ReadAsByteArrayAsync().Result;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
        return content;
    }


}