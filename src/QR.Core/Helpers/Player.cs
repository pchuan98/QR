using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QR.Core.Helpers;

public static class Player
{
    // TODO 之后这里要改成其他库启动，不再包含WPF库
    static MediaPlayer player = new MediaPlayer();

    static string path = Path.GetTempPath() + "temp_voice.mp3";
  

    /// <summary>
    /// 对实际音频播放
    /// </summary>
    /// <param name="voice"></param>
    public static void PlayAsync(byte[]? voice)
    {
        if (voice == null)
        {
            return;
        }
        player.Close();

        if (SafeWriteAllBytes(path, voice))
        {
            player.Open(new Uri(path, UriKind.Relative));
            player.Play();
        }
    }

    static System.IO.Stream? stream = null;
    static System.IO.BinaryWriter? writer = null;

    private static bool SafeWriteAllBytes(string path, byte[] voice)
    {
        try
        {
            if (stream != null || writer != null)
            {
                stream?.Close();
                writer?.Close();
            }

            stream = new FileStream(path, FileMode.Create);
            writer = new BinaryWriter(stream);

            writer.Write(voice);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}