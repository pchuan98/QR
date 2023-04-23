using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR.Core.Models;
using QR.Core.Services;

namespace QR.ViewModels
{
    public class ExtendViewModel : ObservableObject
    {
        public AsyncRelayCommand MergeWordsFilesCommand { get; set; }
        public AsyncRelayCommand UniqueWordsFilesCommand { get; set; }
        public AsyncRelayCommand DownloadWordsCommand { get; set; }
        public AsyncRelayCommand WordsFile2CSVFileCommand { get; set; }
        public AsyncRelayCommand CSVFile2WordsFileCommand { get; set; }
        public AsyncRelayCommand ResetWordsCommand { get; set; }

        public AsyncRelayCommand UpsetWordsCommand { get; set; }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, ResetMessage(value));
        }

        private string ResetMessage(string value)
        {
            var lines = value.Split(new char[] { '\n' });
            if (lines.Length > 500)
            {
                var temp = lines.Skip(300).ToList();
                return String.Join("\n", temp);
            }
            return value;
        }

        private string AddMessage(string value)
            => Message += value + "\n";

        private string AddErrorMessage(string value)
             => Message += DateTime.Now.ToString("t") + "\t" + value + "\n";

        private string AddLineMessage(string value)
        {
            var lines = Message.Split(new char[] { '\n' });
            lines[lines.Length - 1] = value + "\n";
            return String.Join("\n", lines);
        }

        public ExtendViewModel()
        {
            MergeWordsFilesCommand = new(() => Task.Run(MergeWordsFiles));
            UniqueWordsFilesCommand = new(() => Task.Run(UniqueWordsFiles));
            DownloadWordsCommand = new(() => Task.Run(DownloadWords));
            ResetWordsCommand = new(() => Task.Run(ResetWords));
            WordsFile2CSVFileCommand = new(() => Task.Run(WordsFile2CSVFile));
            CSVFile2WordsFileCommand = new(() => Task.Run(CSVFile2WordsFile));

            UpsetWordsCommand = new(() => Task.Run(UpsetWords));
        }

        private static readonly string WordFilterString = "words files(*.words)|*.words|所有文件|*.*";
        private static readonly string CSVFilterString = "csv files(*.csv)|*.csv|所有文件|*.*";

        /// <summary>
        /// 选择多个words合并成单个words文件，自带去重
        /// </summary>
        private void MergeWordsFiles()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = WordFilterString,
                Title = "选择待合并的所有words文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false || odlg.FileNames.Length == 0)
            {
                AddMessage("没有待合并的words文件");
                return;
            }

            var openFiles = odlg.FileNames;

            var merge = new List<MetaWord>();

            AddMessage(String.Format("合并中......"));

            for (int i = 0; i < openFiles.Length; i++)
            {
                try
                {
                    FileService.ReadWords(openFiles[i], out var collection);
                    if (collection == null) throw new Exception("当前文件合并失败:" + openFiles[i]);
                    collection.ForEach(item => merge.Add(new("")
                    {
                        Word = item.Word,
                        Interpretions = item.Interpretions,
                        Voices = item.Voices
                    }));
                }
                catch (Exception e)
                {
                    AddErrorMessage(openFiles[i]);
                    AddErrorMessage(e.Message);
                }
            }


            var sdlg = new SaveFileDialog()
            {
                Filter = WordFilterString,
                Title = "选择合并words文件存储位置"
            };

            var srecall = sdlg.ShowDialog();

            if (srecall == null || srecall == false || String.IsNullOrEmpty(sdlg.FileName))
            {
                AddErrorMessage("没有保存合并的words文件");
                return;
            }
            else FileService.WriteWords(sdlg.FileName, merge);

            // 输出信息，合并了几个文件，分别是谁，合并的条目，保存的位置这些信息
            AddMessage("===============================================");
            AddMessage(String.Format("保存成功：{0}", sdlg.FileName));
            AddMessage(String.Format("总计合并了{0}个文件：", odlg.FileNames.Length));
            for (int i = 0; i < odlg.FileNames.Length; i++)
            {
                AddMessage(String.Format(" - {0}", odlg.FileNames[i]));
            }
            AddMessage(String.Format("总计合并了单词{0}个。", merge.Count));
            AddMessage("===============================================");
        }

        /// <summary>
        /// 选定一个待操作的words，然后选择多个参考words
        /// </summary>
        private void UniqueWordsFiles()
        {
            var gdlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = WordFilterString,
                Title = "选择待去重的words文件"
            };

            var grecall = gdlg.ShowDialog();

            if (grecall == null || grecall == false)
            {
                AddMessage("没有选择待去重文件");
                return;
            }

            AddMessage(String.Format("选择待去重文件:{0}", gdlg.FileName));

            var rdlg = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = WordFilterString,
                Title = "选择去重参考文件"
            };

            var rrecall = rdlg.ShowDialog();

            AddMessage("整理数据中...");

            if (rrecall == null || rrecall == false || rdlg.FileNames.Length == 0) AddMessage("当前没有选择参考文件");


            FileService.ReadWords(gdlg.FileName, out var words);

            var merge = new List<MetaWord>();
            merge.AddRange(words);

            for (int i = 0; i < rdlg.FileNames.Length; i++)
            {
                try
                {
                    FileService.ReadWords(rdlg.FileNames[i], out var collection);
                    if (collection == null) throw new Exception("当前文件参考失败:" + rdlg.FileNames[i]);
                    else merge.AddRange(collection);
                }
                catch (Exception e)
                {
                    AddErrorMessage(rdlg.FileNames[i]);
                    AddErrorMessage(e.Message);
                }
            }
            AddMessage("去重中...");

            // 开始去重
            var unique = new List<MetaWord>();

            for (int i = 0; i < words.Count; i++)
            {
                bool flag = true;
                for (int j = i + 1; j < merge.Count; j++)
                    if (merge[i].Word == merge[j].Word)
                    {
                        flag = false;
                        break;
                    }

                if (flag) unique.Add(words[i]);
            }

            AddMessage("保存去重文件中...");

            var sdlg = new SaveFileDialog()
            {
                Filter = WordFilterString,
                Title = "选择保存取出的words文件",
            };

            var srecall = sdlg.ShowDialog();

            if (srecall == null || srecall == false)
            {
                AddMessage("没有保存去重的words文件");
                return;
            }
            else FileService.WriteWords(sdlg.FileName, unique);

            AddMessage("===============================================");
            AddMessage(String.Format("去重成功:{0}", gdlg.FileName));
            AddMessage(String.Format("总计参考了:{0}个文件:", rdlg.FileNames.Length));
            for (int i = 0; i < rdlg.FileNames.Length; i++)
            {
                AddMessage(String.Format(" - {0}", rdlg.FileNames[i]));
            }
            AddMessage(String.Format("总计去重了单词{0}个。", words.Count - unique.Count));
            AddMessage("===============================================");
        }

        /// <summary>
        /// 选择一个words文件然后download所有的words
        /// </summary>
        private void DownloadWords()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = WordFilterString,
                Title = "选择下载信息的words文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false)
            {
                AddMessage("没有选择待下载信息的words文件");
                return;
            }


            AddMessage("下载中...");
            int count = 0;

            var words = new List<MetaWord>();

            try
            {
                FileService.ReadWords(odlg.FileName, out words);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("打开文件失败 : " + odlg.FileName);
                return;
            }

            foreach (var item in words)
            {
                if (item.IsFull) continue;

                try
                {
                    var w = Core.Helpers.Translator.BingAPI.QuickLoad(item.Word);
                    item.Word = w.Word;
                    item.Interpretions = w.Interpretions;
                    item.Voices = w.Voices;

                    count++;
                    AddMessage(String.Format("[{0}] 下载单词 : {0}",count, item.Word));

                }
                catch (Exception)
                {
                    AddErrorMessage(String.Format("下载失败 : {0}", item.Word));
                }
            }

            AddMessage("===============================================");
            AddMessage("写入文件中......");
            FileService.WriteWords(odlg.FileName, words);
            AddMessage(String.Format("写入完成,{0}", count));
            AddMessage(String.Format("总计更新了单词信息 : {0}", odlg.FileName));
            AddMessage("===============================================");
        }

        /// <summary>
        /// 重置单词信息
        /// </summary>
        private void ResetWords()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = WordFilterString,
                Title = "选择重置信息的words文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false)
            {
                AddMessage("没有选择待下载重置信息的words文件");
                return;
            }


            AddMessage("下载中...");
            int count = 0;
            FileService.ReadWords(odlg.FileName, out var words);

            foreach (var item in words)
            {
                try
                {
                    var w = Core.Helpers.Translator.BingAPI.QuickLoad(item.Word);
                    item.Word = w.Word;
                    item.Interpretions = w.Interpretions;
                    item.Voices = w.Voices;

                    AddMessage(String.Format("[{0}] 下载单词 : {0}", count, item.Word));
                    count++;
                }
                catch (Exception)
                {
                    AddErrorMessage(String.Format("下载失败 : {0}", item.Word));
                }
            }

            AddMessage("===============================================");
            AddMessage("写入文件中......");
            FileService.WriteWords(odlg.FileName, words);
            AddMessage("写入完成.");
            AddMessage(String.Format("总计重置了单词信息 : {0}", count));
            AddMessage("===============================================");
        }

        /// <summary>
        /// 选择一个words文件然后保存成csv文件
        /// </summary>
        private void WordsFile2CSVFile()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = WordFilterString,
                Title = "选择待转换的words文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false)
            {
                AddMessage("选择待转换的words文件");
                return;
            }

            AddMessage("转换中...");
            FileService.ReadWords(odlg.FileName, out var words);

            var csv = new List<string>();
            words.ForEach(item => csv.Add(item.CSVString));

            var merge = String.Join("\n", csv);
            AddMessage("保存转换文件中...");

            var sdlg = new SaveFileDialog()
            {
                Filter = CSVFilterString,
                Title = "选择csv文件存储位置"
            };

            var srecall = sdlg.ShowDialog();

            if (srecall == null || srecall == false || String.IsNullOrEmpty(sdlg.FileName))
            {
                AddErrorMessage("没有保存csv文件");
                return;
            }
            else System.IO.File.WriteAllText(sdlg.FileName, merge);

            AddMessage("===============================================");
            AddMessage(String.Format("保存成功：{0}", sdlg.FileName));
            AddMessage(String.Format("总计转换了单词{0}个。", words.Count));
            AddMessage("===============================================");

        }

        /// <summary>
        /// 打开一个csv文件然后保存成words文件
        /// 这里要求csv必须在第一个位置
        /// </summary>
        private void CSVFile2WordsFile()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = CSVFilterString,
                Title = "选择待转换的csv文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false)
            {
                AddMessage("选择待转换的csv文件");
                return;
            }

            AddMessage("转换中...");
            var content = System.IO.File.ReadAllText(odlg.FileName);

            var words = new List<MetaWord>();
            var lines = content.Split(new char[] { '\n' });

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (String.IsNullOrEmpty(line)) continue;

                try
                {
                    var word = line.Split(new char[] { ',' })[0];
                    word = word.Replace("\n", "");
                    word = word.Trim();
                    words.Add(new MetaWord(word));
                }
                catch (Exception)
                {
                    AddErrorMessage(String.Format("无法转换的行 : {0}", line));
                }
            }

            AddMessage("保存转换文件中...");

            var sdlg = new SaveFileDialog()
            {
                Filter = WordFilterString,
                Title = "选择words文件存储位置"
            };

            var srecall = sdlg.ShowDialog();

            if (srecall == null || srecall == false || String.IsNullOrEmpty(sdlg.FileName))
            {
                AddErrorMessage("没有保存words文件");
                return;
            }
            else FileService.WriteWords(sdlg.FileName, words);

            AddMessage("===============================================");
            AddMessage(String.Format("保存成功：{0}", sdlg.FileName));
            AddMessage(String.Format("总计转换了单词{0}个。", words.Count));
            AddMessage("===============================================");
        }

        /// <summary>
        /// 打乱单词
        /// </summary>
        private void UpsetWords()
        {
            var odlg = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = WordFilterString,
                Title = "选择待打乱的words文件"
            };

            var orecall = odlg.ShowDialog();

            if (orecall == null || orecall == false)
            {
                AddMessage("选择待打乱的words文件");
                return;
            }

            AddMessage("打乱中...");
            var words = new List<MetaWord>();
            try
            {
                FileService.ReadWords(odlg.FileName, out words);
                Core.Helpers.ListHelper<MetaWord>.Random(words);
            }
            catch (Exception e)
            {
                AddErrorMessage(e.Message);
                return;
            }

            AddMessage("===============================================");
            AddMessage("保存打乱后的words文件中...");
            AddMessage("写入文件中......");
            FileService.WriteWords(odlg.FileName, words);
            AddMessage("写入完成.");
            AddMessage("===============================================");

            
        }
    }
}