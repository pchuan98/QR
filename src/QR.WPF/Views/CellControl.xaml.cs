using QR.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QR.WPF
{
    /// <summary>
    /// CellControl.xaml 的交互逻辑
    /// </summary>
    public partial class CellControl : UserControl
    {



        public static Stretch GetViewBoxStretch(DependencyObject obj)
        {
            return (Stretch)obj.GetValue(ViewBoxStretchProperty);
        }

        public static void SetViewBoxStretch(DependencyObject obj, Stretch value)
        {
            obj.SetValue(ViewBoxStretchProperty, value);
        }

        // Using a DependencyProperty as the backing store for ViewBoxStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewBoxStretchProperty =
            DependencyProperty.RegisterAttached("ViewBoxStretch", typeof(Stretch), typeof(CellControl), new PropertyMetadata(Stretch.Uniform));





        public static double GetCellFontSize(DependencyObject obj)
        {
            return (double)obj.GetValue(CellFontSizeProperty);
        }

        public static void SetCellFontSize(DependencyObject obj, double value)
        {
            obj.SetValue(CellFontSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellFontSizeProperty =
            DependencyProperty.RegisterAttached("CellFontSize", typeof(double), typeof(CellControl), new PropertyMetadata(12.0));




        public static double GetCellWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(CellWidthProperty);
        }

        public static void SetCellWidth(DependencyObject obj, double value)
        {
            obj.SetValue(CellWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellWidthProperty =
            DependencyProperty.RegisterAttached("CellWidth", typeof(double), typeof(CellControl), new PropertyMetadata(0.0));




        public static double GetCellHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(CellHeightProperty);
        }

        public static void SetCellHeight(DependencyObject obj, double value)
        {
            obj.SetValue(CellHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for CellHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellHeightProperty =
            DependencyProperty.RegisterAttached("CellHeight", typeof(double), typeof(CellControl), new PropertyMetadata(0.0));




        public CellControl()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) => ((ViewModels.CellViewModel)DataContext)?.Refresh();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            => ((CellViewModel)DataContext)?.Glance();

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
            => ((CellViewModel)DataContext)?.Speak();
    }
}
