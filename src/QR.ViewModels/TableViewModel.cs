using CommunityToolkit.Mvvm.ComponentModel;
using QR.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.ViewModels;

public class TableViewModel : ObservableObject
{
    /// <summary>
    /// 显示集合
    /// </summary>
    public ObservableCollection<MetaWord> WordCollection { get; set; } = new();

    public TableViewModel() { }
}