using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QR.WPF
{
    /// <summary>
    /// PrintWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();

            t_msg.TextChanged += T_msg_TextChanged;
        }

        private void T_msg_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 将就用着吧
            t_msg.CaretIndex = t_msg.Text.Length;
            t_msg.ScrollToEnd();
        }
    }
}
