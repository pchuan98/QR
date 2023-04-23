using QR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace QR.Core.Services;

[Flags]
public enum DlgType
{
    OpenDlg = 0,
    SaveDlg = 1,
}

public static class FileService
{
    /// <summary>
    /// 读取words文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="words"></param>
    public static void ReadWords(string path, out List<MetaWord> words)
    {
        words = new();

        if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
            throw new Exception("Path is invalid.");

        path = path.Trim();

        var compress = System.IO.File.ReadAllBytes(path);
        System.IO.MemoryStream ms = new(compress);
        System.IO.Compression.GZipStream compressedzipStream
            = new(ms, System.IO.Compression.CompressionMode.Decompress);
        System.IO.MemoryStream outBuffer = new();
        byte[] block = new byte[1024];
        while (true)
        {
            int bytesRead = compressedzipStream.Read(block, 0, block.Length);
            if (bytesRead <= 0)
                break;
            else
                outBuffer.Write(block, 0, bytesRead);
        }
        compressedzipStream.Close();
        var decompress = outBuffer.ToArray();

        var js = System.Text.Encoding.UTF8.GetString(decompress);

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MetaWord>>(js);

        if (result == null) throw new Exception("解析词典数据错误");

        words = result;
    }


    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="words"></param>
    public static void WriteWords(string path, List<MetaWord> words)
    {
        if (string.IsNullOrEmpty(path))
            throw new Exception("Path is invalid.");

        // 将word转成json
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(words);

        // 将json文件二进制化
        var bs = System.Text.Encoding.UTF8.GetBytes(json);

        // 将二进制文件压缩
        System.IO.MemoryStream ms = new();
        System.IO.Compression.GZipStream compressedzipStream
            = new(ms, System.IO.Compression.CompressionMode.Compress, true);
        compressedzipStream.Write(bs, 0, bs.Length);
        compressedzipStream.Close();
        var compress = ms.ToArray();

        // 将压缩过的二进制文件保存
        System.IO.File.WriteAllBytes(path, compress);
    }

    /// <summary>
    /// 显示文件弹窗
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filter"></param>
    /// <param name="title"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static bool ShowFileDialog(
        out string path,
        string filter = "words files(*.words)|*.words|所有文件|*.*",
        string title = ".words文件",
        DlgType dt = DlgType.OpenDlg
        )
    {
        path = ValueBox.EmptyString;

        FileDialog dlg = dt == DlgType.OpenDlg ? new OpenFileDialog() : new SaveFileDialog();
        dlg.Filter = filter;
        dlg.Title = title;

        var result = dlg.ShowDialog();
        if (result == null || result == false) return false;

        path = dlg.FileName;
        return true;
    }

    /// <summary>
    /// 显示读文件多文件弹窗
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filter"></param>
    /// <param name="title"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static bool ReadFilesDialog(
        out List<string> path,
        string filter = "words files(*.words)|*.words|所有文件|*.*",
        string title = ".words文件")
    {
        path = new();

        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = filter;
        dlg.Title = title;
        dlg.Multiselect = true;

        var result = dlg.ShowDialog();
        if (result == null || result == false) return false;

        path = dlg.FileNames.ToList();
        return true;
    }
}