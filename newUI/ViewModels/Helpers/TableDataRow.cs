using System.Collections.Generic;

namespace newUI.ViewModels.Helpers;

public class TableDataRow<TCell, TColumn, TRow> : ViewModelBase where TCell : ViewModelBase where TColumn : notnull
{
    private Dictionary<TColumn, TCell> cells = new();
    private TRow rowHeader;

    public TableDataRow(TRow rowHeader)
    {
        this.rowHeader = rowHeader;
    }

    public TRow RowHeader
    {
        get => rowHeader;
        set => SetProperty(ref rowHeader, value, nameof(rowHeader));
    }
    
    public void SetCell(TColumn column, TCell cell)
    {
        cells[column] = cell; //не будет ли тут ошибки?
        OnPropertyChanged(nameof(cells)); 
    }
    
    public TCell? this[TColumn column]
    {
        get => cells.TryGetValue(column, out var cell) ? cell : default;
        set => SetCell(column, value!);
    }
}