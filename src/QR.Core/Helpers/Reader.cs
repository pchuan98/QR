using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace QR.Core.Helpers;

public static class Reader
{
    static Prompt? prompt = null;

    static SpeechSynthesizer reader = new SpeechSynthesizer();

    /// <summary>
    /// 调用Synthesis合成语音
    /// </summary>
    /// <param name="text"></param>
    public static void ReadStringAsync(string text)
    {
        if (prompt != null) reader.SpeakAsyncCancel(prompt);
        prompt = reader.SpeakAsync(text);
    }
}