using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace newUI.ViewModels.Helpers;

public class TableDataRow<TCell, TColumn, TRow> : ViewModelBase where TCell : ViewModelBase where TColumn : notnull
{
    private Dictionary<TColumn, TCell> cells = new();
    private List<TCell> orderedCells = new();
    private TRow rowHeader;
    private List<TColumn> columnOrder;

    public TableDataRow(TRow rowHeader)
    {
        this.rowHeader = rowHeader;
    }
    
    public ObservableCollection<TCell> Cells 
    {
        get
        {
            orderedCells.Clear();
            foreach (var column in columnOrder)
            {
                if (cells.TryGetValue(column, out var cell))
                {
                    orderedCells.Add(cell);
                }
            }
            return new ObservableCollection<TCell>(orderedCells);
        }
    }
    
    public Dictionary<TColumn, TCell> CellDictionary => cells;

    public TRow RowHeader
    {
        get => rowHeader;
        set => SetProperty(ref rowHeader, value, nameof(rowHeader));
    }
    
    public void SetCell(TColumn column, TCell cell)
    {
        cells[column] = cell;
        if (!columnOrder.Contains(column))
        {
            columnOrder.Add(column);
        }
        OnPropertyChanged(nameof(Cells));
    }
    
    public TCell? this[TColumn column]
    {
        get => cells.TryGetValue(column, out var cell) ? cell : default;
        set => SetCell(column, value!);
    }
    
    public void UpdateColumnOrder(List<TColumn> newColumnOrder)
    {
        columnOrder = newColumnOrder;
        OnPropertyChanged(nameof(Cells));
    }
}