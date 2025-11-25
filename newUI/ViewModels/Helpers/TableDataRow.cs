using System.Collections.Generic;

namespace newUI.ViewModels.Helpers;

public class TableDataRow<TCell, TColumn, TRow> : ViewModelBase where TCell : ViewModelBase where TColumn : notnull
{
    private Dictionary<TColumn, TCell> cells = new();
    private TRow rowHeader;

    public TRow RowHeader
    {
        get => rowHeader;
        set => SetProperty(ref rowHeader, value, nameof(rowHeader));
    }
    
    public void AddCell(TColumn column, TCell cell)
    {
        cells.Add(column, cell);
        OnPropertyChanged(nameof(cells));
    }
    
    public TCell? this[TColumn column] => 
        cells.TryGetValue(column, out var cell) 
            ? cell 
            : default;
}