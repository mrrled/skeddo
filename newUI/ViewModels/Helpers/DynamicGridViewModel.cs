using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace newUI.ViewModels.Helpers;

public class DynamicGridViewModel<TCell, TColumn, TRow> : ViewModelBase where TRow : notnull
{
    public ObservableCollection<DataRow> Rows { get; } = new();
    public ObservableCollection<TColumn> Columns { get; } = new();

    public DynamicGridViewModel()
    {
        
    }

    public void LoadDataFromBackend(Func<List<(TColumn, Dictionary<TRow, TCell>)>> dataLoader)
    {
        var data = dataLoader();
        Rows.Clear();
        Columns.Clear();
        if (data.Count == 0) return;
        
        
    }
}