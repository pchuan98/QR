using QR.Core.Helpers.PDF;
using QR.Core.Services;
using QuestPDF.Fluent;
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
    /// GridWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GridWindow : Window
    {
        public ViewModels.SearchViewModel? SearchVM = null;
        private DispatcherTimer VoiceTimer;

        public GridWindow()
        {
            InitializeComponent();


            // 防止渲染出错
            try
            {
                // 主题
                foreach (var child in mi_theme.Items)
                {
                    if (child is MenuItem mi) mi.IsChecked = false;
                }
                ((MenuItem)mi_theme.Items[Properties.Settings.Default.Theme]).IsChecked = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            this.SizeChanged += GridWindow_SizeChanged;

            try
            {
                this.Width = Properties.WindowCommonConfig.Default.GridWindowWidth;
                this.Height = Properties.WindowCommonConfig.Default.GridWindowHeight;
            }
            catch (Exception) { }

            this.Closed += (s, e) =>
            {
                try
                {
                    Properties.WindowCommonConfig.Default.GridWindowWidth = this.Width;
                    Properties.WindowCommonConfig.Default.GridWindowHeight = this.Height;
                    Properties.WindowCommonConfig.Default.Save();
                }
                catch (Exception) { }
            };

            VoiceTimer = new(priority: DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromSeconds(s_voice_tick.Value),
            };

            // 还有一些其他行为要中断Timer  懒得写了

            VoiceTimer.Tick += (s, e) =>
            {
                if (this.DataContext != null && this.DataContext is ViewModels.GridViewModel gvm)
                {

                    while (VoiceIndex < gvm.ShowCollection.Count - 1
                        && gvm.ShowCollection[VoiceIndex].Meta.Voices == null) VoiceIndex++;

                    if (VoiceIndex > gvm.ShowCollection.Count - 1)
                    {
                        // 如果还有其他page，那么就更换到下一个page
                        if (gvm.Page == gvm.MaxPage)
                        {
                            if (VoiceTrueRepeatCount >= VoiceRepeatCount - 1)
                            {
                                OnVoiceStop();
                                return;
                            }

                            VoiceIndex = 0;
                            VoiceTrueRepeatCount++;
                        }
                        // 这里在处理Repeat的逻辑的时候有bug  没时间改了
                        else
                        {
                            if (VoiceTrueRepeatCount >= VoiceRepeatCount - 1) gvm.Page++;

                            VoiceIndex = 0;
                            VoiceTrueRepeatCount++;
                        }
                    }


                    gvm.ShowCollection[VoiceIndex].Speak();
                    VoiceIndex++;
                }
            };

            s_voice_tick.ValueChanged += (s, e)
                => VoiceTimer.Interval = TimeSpan.FromSeconds(s_voice_tick.Value);

            s_voice_repeat.ValueChanged += (s, e)
                => VoiceRepeatCount = (int)s_voice_repeat.Value;
        }

        private void GridWindow_SizeChanged(object sender, SizeChangedEventArgs e)
            => ResetCellSize();

        private void ResetCellSize()
        {
            //if (this.DataContext != null && this.DataContext is ViewModels.GridViewModel gvm && gvm != null)
            //{
            //    var height = Body.ActualHeight / gvm.Rows;
            //    var width = Body.ActualWidth / gvm.Columns;

            //    tag_itemControls.Height = height - 5;
            //    tag_itemControls.Width = width - 5;
            //}
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

        private void MenuItem_Click_OpenSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchWindow window = new();
                window.DataContext = SearchVM;
                window.ResetDataContext(SearchVM);
                window.SkipReloadSearchWindow = true;
                window.SkipArgs = true;
                window.SkipReadTempFile = true;
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void MenuItem_Click_OpenPrint(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintWindow window = new();

                if (DataContext == null)
                {
                    MessageBox.Show("GridView的DataContext为空");
                    return;
                }

                ViewModels.PrintViewModel vm = new((ViewModels.GridViewModel)this.DataContext);
                window.DataContext = vm;
                window.Topmost = false;

                window.ShowDialog();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (tb_full.IsChecked != null && (bool)tb_full.IsChecked)
            {
                tag.Visibility = Visibility.Collapsed;
                tag.IsEnabled = false;

                this.Left = 0.0;
                this.Top = 0.0;
                this.Width = SystemParameters.PrimaryScreenWidth;
                this.Height = SystemParameters.PrimaryScreenHeight;


            }
            else
            {
                tag.Visibility = Visibility.Visible;
                tag.IsEnabled = true;


                this.Width = Properties.WindowCommonConfig.Default.GridWindowWidth;
                this.Height = Properties.WindowCommonConfig.Default.GridWindowHeight;

                this.Top = (System.Windows.SystemParameters.PrimaryScreenHeight - Height) / 2;
                this.Left = (System.Windows.SystemParameters.PrimaryScreenWidth - Width) / 2;
            }
        }

        private void MenuItem_Click_OpenTableWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                TableWindow window = new();
                ViewModels.TableViewModel vm = new();
                vm.WordCollection = new(((ViewModels.GridViewModel)this.DataContext).WordCollection);
                window.DataContext = vm;
                window.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void MenuItem_Click_OpenExtendWindow(object sender, RoutedEventArgs e)
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

        private bool IsPlayed = false;
        private int VoiceIndex = 0;
        private int VoiceRepeatCount = 1;
        private int VoiceTrueRepeatCount = 0;

        private void bt_voice_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPlayed) OnVoicePlay();
            else OnVoiceStop();
        }

        private void OnVoicePlay()
        {
            IsPlayed = true;
            bt_voice.Content = "Stop";
            VoiceTimer.Start();
        }

        private void OnVoiceStop()
        {
            IsPlayed = false;
            bt_voice.Content = "Play";
            VoiceTimer.Stop();

            VoiceIndex = 0;
            VoiceTrueRepeatCount = 0;
        }

        bool IsSingleMode = false;

        /// <summary>
        /// 单例模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsSingleMode = !IsSingleMode;
            if (this.DataContext is ViewModels.GridViewModel gvm)
            {
                if (IsSingleMode)
                {
                    mi_mode.Header = "多例模式";
                    gvm.Rows = 1;
                    gvm.Columns = 1;

                    s_fontsize.Value = s_fontsize.Maximum;
                }

                else
                {
                    mi_mode.Header = "单例模式";
                    gvm.Rows = 18;
                    gvm.Columns = 6;

                    s_fontsize.Value = 18;
                }
            }


        }

        /// <summary>
        /// 单独来的一个打印模式，主要是把所有的单词一次性输出到一张纸上
        /// 是后期复习用的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_print_Click(object sender, RoutedEventArgs e)
        {
            // 需要的数据就是当前所有的单词，column和row
            if (this.DataContext is ViewModels.GridViewModel gvm)
            {
                int rows = gvm.Rows;
                int columns = gvm.Columns;

                List<string> words = new();
                gvm.ShowCollection.ForEach(item => words.Add(item.Meta.Word));

                try
                {
                    var document = new SinglePDFDocument();

                    // 形状参数
                    document.TableColumns = columns;
                    document.TableRows = rows;

                    // 给数据生成
                    document.SourceData = words;
                    document.Render();

                    // 保存
                    if (FileService.ShowFileDialog(out var path, "pdf file(*.pdf)|*.pdf|所有文件(*.*)|*.*", "保存PDF文件", DlgType.SaveDlg))
                    {
                        document.Save(path);
                        System.Diagnostics.Process.Start("Explorer.exe", path);
                    }

                }
                catch (Exception exception) { MessageBox.Show(exception.Message); }
            }



        }
    }


    public class SinglePDFDocument : BaseDocumentView
    {

        public override List<TableModel> CreateTableModels()
            => new() { new()
            {
                RowDefinitions=TableRows,
                ColumnDefinitions=TableColumns,
                Data = Skim(SourceData).ToArray(),
                TableDefaultFontSize = FontSize,
                TableDefaultFontFamily = FontFamily
            } };

        public override void OnInitialized()
        {
            GridColumns = 1;
            GridRows = 1;

            float margin = 20.0f;

            PageMargin = new float[] { margin, margin, margin, margin };

            GridVerticalSpacing = 0;
            GridHorizontalSpacing = 0;

            FontFamily = "Times New Roman";

            FontSize = 18;
        }

        private List<string> Skim(List<string> data)
        {
            int total = this.TableColumns * this.TableRows;
            int count = data.Count;

            if (count > total)
                return QR.Core.Helpers.ListHelper<string>.Split(data, total);
            else
                for (int i = 0; i < total - count; i++)
                    data.Add("");
            return data;
        }
    }
}
