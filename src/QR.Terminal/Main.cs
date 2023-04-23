using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Terminal;

internal class Program
{
    static void Main(string[] args)
    {
        // 必须设置，不然音标会乱码
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (args.Length >= 2)
        {
            var param = args[0];
            if (param == "search" || param == "s")
            {
                for (int i = 0; i < args.Length - 1; i++)
                {
                    Console.WriteLine(Core.Helpers.Translator.BingAPI.QuickLoad(args[i + 1]));
                    Console.WriteLine("--------------------------------------------------------------------");
                }
            }
        }
        else
        {
            Console.WriteLine("wqr search[s] [w1 w2 ...]");
        }
    }
}