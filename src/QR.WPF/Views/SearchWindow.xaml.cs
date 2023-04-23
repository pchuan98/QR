using CommunityToolkit.Mvvm.Input;
using HandyControl.Data;
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
using System.Windows.Threading;

namespace QR.WPF
{
    /// <summary>
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : Window
    {
        ViewModels.SearchViewModel VM = new();

        // 防止死锁
        public bool SkipReloadSearchWindow = false;

        public bool SkipReadTempFile = false;

        public bool SkipArgs = false;

        String[] arguments = Environment.GetCommandLineArgs();

        public SearchWindow()
        {
            InitializeComponent();


            DataContext = VM;

            view.SelectionChanged += (s, e) =>
            {
                VM.ShowWord = (QR.Core.Models.MetaWord)((DataGrid)s).SelectedItem;
            };

            view.GotFocus += (s, e) => view.UnselectAll();

            search.PreviewMouseLeftButtonUp += (s, e) => search.SelectAll();
            search.SearchStarted += (s, e) => search.SelectAll();

            search.MouseRightButtonUp += (s, e) =>
            {
                search.SelectAll();
                e.Handled = true;
            };

            this.Loaded += SearchWindow_Loaded;

            try
            {
                this.Width = Properties.WindowCommonConfig.Default.SearchWindowWidth;
                this.Height = Properties.WindowCommonConfig.Default.SearchWindowHeight;
            }
            catch (Exception) { }

            this.Closed += (s, e) =>
            {
                try
                {
                    Properties.WindowCommonConfig.Default.SearchWindowWidth = this.Width;
                    Properties.WindowCommonConfig.Default.SearchWindowHeight = this.Height;
                    Properties.WindowCommonConfig.Default.Save();
                }
                catch (Exception) { }
            };
        }

        public void ResetDataContext(ViewModels.SearchViewModel? vm)
        {
            if (vm == null) return;
            VM = vm;
        }

        /// <summary>
        /// 有一定概率卡配置文件,用try来直接解决
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置主题最优先
            try
            {
                // 主题
                foreach (var child in mi_theme.Items) if (child is MenuItem mi) mi.IsChecked = false;
                var theme = ((MenuItem)mi_theme.Items[Properties.Settings.Default.Theme]);
                theme.IsChecked = true;
                ThemeHelper.UpdateGlobalSkin((byte)Properties.Settings.Default.Theme);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            if (!SkipArgs)
            {
                try
                {
                    if (arguments.Length == 2)
                    {
                        var path = arguments[1];

                        VM.ReadWords(path);

                        SkipReloadSearchWindow = true;
                        SkipReadTempFile = true;

                        CreateTableWindow();

                        Close();
                        return;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }


            try
            {
                // 主题
                foreach (var child in mi_theme.Items) if (child is MenuItem mi) mi.IsChecked = false;
                var theme = ((MenuItem)mi_theme.Items[Properties.Settings.Default.Theme]);
                theme.IsChecked = true;
                ThemeHelper.UpdateGlobalSkin((byte)Properties.Settings.Default.Theme);

                // 临时文件
                if (Properties.Settings.Default.StartWithTemp)
                {
                    if (!SkipReadTempFile) VM.ReadWords(ViewModels.SearchViewModel.TempFilePath);
                    ((MenuItem)mi_temp.Items[0]).IsChecked = true;
                }
                else ((MenuItem)mi_temp.Items[1]).IsChecked = true;

                // 打开窗口
                if (Properties.Settings.Default.LaunchWindow == 0) ((MenuItem)mi_window.Items[0]).IsChecked = true;
                else
                {
                    ((MenuItem)mi_window.Items[1]).IsChecked = true;

                    if (!SkipReloadSearchWindow)
                    {
                        GridWindow window = new();
                        window.DataContext = new ViewModels.GridViewModel(VM.WordCollection.ToList());
                        window.SearchVM = VM;
                        window.Show();

                        this.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void MenuItem_Click_SwitchTheme(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                // 设置主题
                switch (item.Header)
                {
                    case "Light":
                        ThemeHelper.UpdateGlobalSkin(0);
                        Properties.Settings.Default.Theme = 0;
                        break;
                    case "Dark":
                        ThemeHelper.UpdateGlobalSkin(1);
                        Properties.Settings.Default.Theme = 1;
                        break;
                    case "Violet":
                        ThemeHelper.UpdateGlobalSkin(2);
                        Properties.Settings.Default.Theme = 2;
                        break;
                    default:
                        break;
                }

                try
                {
                    Properties.Settings.Default.Save();

                    var father = (MenuItem)item.Parent;

                    foreach (var child in father.Items)
                    {
                        if (child is MenuItem mi) mi.IsChecked = false;
                    }

                    item.IsChecked = true;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }


        private void MenuItem_Click_WordsTableWindow(object sender, RoutedEventArgs e)
            => CreateTableWindow();

        private void CreateTableWindow()
        {
            try
            {
                TableWindow window = new();
                ViewModels.TableViewModel vm = new();
                vm.WordCollection = ((ViewModels.SearchViewModel)this.DataContext).WordCollection;
                window.DataContext = vm;
                window.SearchDataGrid = view;
                window.LastViewModel = DataContext;
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void MenuItem_Click_RememberWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                GridWindow window = new();
                window.DataContext = new ViewModels.GridViewModel(VM.WordCollection.ToList());
                window.SearchVM = VM;
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void MenuItem_Click_SwitchTempFile(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                var father = (MenuItem)item.Parent;

                foreach (var child in father.Items)
                {
                    if (child is MenuItem mi) mi.IsChecked = false;
                }

                try
                {
                    item.IsChecked = true;

                    if (item.Header != null && (string)item.Header == "开启") Properties.Settings.Default.StartWithTemp = true;
                    else Properties.Settings.Default.StartWithTemp = false;

                    Properties.Settings.Default.Save();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void MenuItem_Click_SwitchWindow(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                var father = (MenuItem)item.Parent;

                foreach (var child in father.Items)
                {
                    if (child is MenuItem mi) mi.IsChecked = false;
                }

                try
                {
                    item.IsChecked = true;

                    if (item.Header != null && (string)item.Header == "查询窗口") Properties.Settings.Default.LaunchWindow = 0;
                    else Properties.Settings.Default.LaunchWindow = 1;

                    Properties.Settings.Default.Save();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void MenuItem_ExtendWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtendWindow window = new();
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }


    
    
}
