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
    /// TableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TableWindow : Window
    {
        public DataGrid? SearchDataGrid = null;

        public object? LastViewModel = null;

        public TableWindow()
        {
            InitializeComponent();

            Loaded += TableWindow_Loaded;


            try
            {
                this.Width = Properties.WindowCommonConfig.Default.TableWindowWidth;
                this.Height = Properties.WindowCommonConfig.Default.TableWindowHeight;
            }
            catch (Exception) { }

            this.Closed += (s, e) =>
            {
                try
                {
                    Properties.WindowCommonConfig.Default.TableWindowWidth = this.Width;
                    Properties.WindowCommonConfig.Default.TableWindowHeight = this.Height;
                    Properties.WindowCommonConfig.Default.Save();
                }
                catch (Exception) { }
            };
        }

        private void TableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.SearchViewModel vm)
            {
                //editor.CellEditEnding += (s, e) => vm.SaveWords();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (editor.SelectedItem is Core.Models.MetaWord word)
            {
                try
                {
                    var temp = Core.Helpers.Translator.BingAPI.QuickLoad(word.Word);

                    word.Word = temp.Word;
                    word.Voices = temp.Voices;
                    word.Interpretions = temp.Interpretions;

                    editor.Items.Refresh();
                    SearchDataGrid?.Items.Refresh();
                }
                catch (Exception) {}
            }
        }

        /// <summary>
        /// 导出所选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var items = new List<QR.Core.Models.MetaWord>();

            foreach (var item in editor.SelectedItems)
            {
                items.Add((Core.Models.MetaWord)item);
            }

            if (Core.Services.FileService.ShowFileDialog(out var path, dt: Core.Services.DlgType.SaveDlg))
            {
                Core.Services.FileService.WriteWords(path, items);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchWindow window = new();
                if (LastViewModel != null && LastViewModel is ViewModels.SearchViewModel vm)
                {
                    window.DataContext = vm;
                    window.ResetDataContext(vm);
                    window.SkipReloadSearchWindow = true;
                    window.SkipArgs = true;
                    window.SkipReadTempFile = true;
                }
                window.Show();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                GridWindow window = new();
                window.DataContext = new ViewModels.GridViewModel(((ViewModels.TableViewModel)DataContext).WordCollection.ToList());
                
                if (LastViewModel != null && LastViewModel is ViewModels.SearchViewModel vm)
                {
                    window.SearchVM = vm;
                }

                window.Show();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModels.TableViewModel vm)
            {
                if (Core.Services.FileService.ShowFileDialog(out var path, dt: Core.Services.DlgType.SaveDlg))
                {
                    Core.Services.FileService.WriteWords(path, vm.WordCollection.ToList());
                }
            }

           
        }
    }
}
