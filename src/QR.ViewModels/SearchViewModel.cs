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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QR.Core.Models;
using QR.Core.Helpers.Translator;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using static System.Collections.Specialized.BitVector32;
using System.IO;

namespace QR.ViewModels;

public class SearchViewModel : ObservableObject
{
    private readonly static string SearchToken = "SearchManager";

    // TODO ����ط�֮����Ū������ʾ��ʵ�ʵ�collection����Ūһ��
    public List<MetaWord> _WordCollction = new();

    /// <summary>
    /// ��ʾ����
    /// </summary>
    public ObservableCollection<MetaWord> WordCollection { get; set; } = new();

    /// <summary>
    /// ���ҵ���
    /// </summary>
    public RelayCommand<string?> SearchCommand { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public RelayCommand<string?> PlayVoiceCommand { get; set; }

    /// <summary>
    /// ��ȡword�ļ�
    /// </summary>
    public RelayCommand ReadWordsCommand { get; set; }

    /// <summary>
    /// ����words�ļ�
    /// </summary>
    public RelayCommand SaveWordsCommand { get; set; }

    /// <summary>
    /// �Զ��������õ�words��Ϣ�������ǰ�汾ʹ��ָ����λ�ã���֧���Զ���
    /// </summary>
    public RelayCommand AutoSaveWordsCommand { get; set; }

    /// <summary>
    /// ����ʱ�ļ�
    /// </summary>
    public RelayCommand ReadTempWordsFileCommand { get; set; }

    /// <summary>
    /// �������ɶ����������ֻ������clearһ��
    /// </summary>
    public RelayCommand ClearCollectionCommand { get; set; }

    /// <summary>
    /// �ļ��򿪵�ַ
    /// </summary>
    private string? OpenFilePath { get; set; } = null;

    /// <summary>
    /// ����һ����ʱ�ļ�
    /// </summary>
    public readonly static string TempFilePath = System.IO.Path.GetTempPath() + "TEMPWORDS.words";

    private MetaWord _word = new("");
    public MetaWord ShowWord
    {
        get => _word;
        set
        {
            SetProperty(ref _word, value);

            if (value == null) return;

            if (value.Voices == null || value.Voices.Count != 2) return;
            else
            {
                Voice1 = value.Voices.ElementAt(0).Key;
                Voice2 = value.Voices.ElementAt(1).Key;
            }
        }
    }

    private string _voice1 = "";
    public string Voice1
    {
        get => _voice1;
        set => SetProperty(ref _voice1, value);
    }

    private string _voice2 = "";
    public string Voice2
    {
        get => _voice2;
        set => SetProperty(ref _voice2, value);
    }

    private bool _autosave = true;
    public bool IsAutoSave
    {
        get => _autosave;
        set => SetProperty(ref _autosave, value);
    }

    private bool _issearcherror = false;
    public bool IsSearchError
    {
        get => _issearcherror;
        set => SetProperty(ref _issearcherror, value);
    }

    private string _errorstr = "";
    public string SearchErrorStr
    {
        get => _errorstr;
        set => SetProperty(ref _errorstr, value);
    }

    public SearchViewModel()
    {

        SearchCommand = new(Search);
        PlayVoiceCommand = new(PlayVoice);
        ReadWordsCommand = new(ReadWordsWithDialog);
        SaveWordsCommand = new(SaveWordsWithDialog);
        AutoSaveWordsCommand = new(() =>
        {
            this.IsAutoSave = !this.IsAutoSave;
            if (this.IsAutoSave) WeakReferenceMessenger.Default.Send("SaveRightNow", SearchToken);

        });

        ReadTempWordsFileCommand = new(() => WeakReferenceMessenger.Default.Send("ReadTempWordsFile", SearchToken));

        ClearCollectionCommand = new(() => WordCollection.Clear());

        WeakReferenceMessenger.Default.Register<string, string>(this, SearchToken, SearchCoreHelper);

    }

    /// <summary>
    /// �ܵ���������Ҫ�����������������������֮��Ķ���
    /// ����ط�ֻ�Ǵ򲹶������漰�������ķ�����ʵ��
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SearchCoreHelper(object recipient, string message)
    {
        // �����Զ��洢�������||����л����˱��棬���̱���һ��
        if ((message == "Search" || message == "SaveRightNow") && IsAutoSave)
            SaveWords(TempFilePath);

        if (message == "ReadTempWordsFile") ReadWords(TempFilePath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Search(string? value)
    {
        if (value == null || String.IsNullOrEmpty(value)) return;

        try
        {
            // ���ô�����Ϣ
            IsSearchError = false;
            SearchErrorStr = "";

            ShowWord = BingAPI.QuickLoad(value);

            // �����Ƿ��Ѿ���ӹ���
            foreach (var item in WordCollection)
            {
                if (item.Word == ShowWord.Word && item.IsFull)
                {
                    ShowWord = item;
                    return;
                }
            }

            WordCollection.Add(ShowWord);

            WeakReferenceMessenger.Default.Send("Search", SearchToken);
        }
        catch (Exception e)
        {
            IsSearchError = true;
            SearchErrorStr = e.Message;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void PlayVoice(string? key)
    {
        if (key == null || String.IsNullOrEmpty(key) || ShowWord.Voices == null) return;

        Core.Helpers.Player.PlayAsync(this.ShowWord.Voices[key]);
    }

    public void ReadWords(string path)
    {
        if (String.IsNullOrEmpty(path)) return;
        try
        {
            Core.Services.FileService.ReadWords(path, out var collection);

            WordCollection.Clear();
            collection.ForEach(item => WordCollection.Add(item));

            OpenFilePath = path;

            if (WordCollection.Count >= 1) ShowWord = WordCollection[0];
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    public void SaveWords(string path)
        => Core.Services.FileService.WriteWords(path, WordCollection.ToList());

    public void SaveWordsWithDialog()
    {
        try
        {
            Core.Services.FileService.ShowFileDialog(out var path, dt: Core.Services.DlgType.SaveDlg);
            SaveWords(path);
        }
        catch (Exception) { }
    }

    public void ReadWordsWithDialog()
    {
        try
        {
            Core.Services.FileService.ShowFileDialog(out var path);
            ReadWords(path);
        }
        catch (Exception) { }
    }
}