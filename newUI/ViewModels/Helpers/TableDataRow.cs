using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace newUI.ViewModels.Helpers;

public class TableDataRow<TCell, TColumn, TRow> : ViewModelBase where TCell : ViewModelBase where TColumn : notnull
{
    private readonly Dictionary<TColumn, int> columnIndexMap = new();
    private readonly ObservableCollection<TCell> cells = new();
    private TRow rowHeader;

    public TableDataRow(TRow rowHeader, IReadOnlyList<TColumn> columns)
    {
        this.rowHeader = rowHeader;
        InitializeCells(columns);
    }
    
    public ObservableCollection<TCell> Cells => cells;
    
    public TRow RowHeader
    {
        get => rowHeader;
        set => SetProperty(ref rowHeader, value);
    }
    
    private void InitializeCells(IReadOnlyList<TColumn> columns)
    {
        columnIndexMap.Clear();
        cells.Clear();
        
        for (int i = 0; i < columns.Count; i++)
        {
            columnIndexMap[columns[i]] = i;
            cells.Add(default);
        }
    }
    
    public void SetCell(TColumn column, TCell cell)
    {
        if (columnIndexMap.TryGetValue(column, out int index))
        {
            cells[index] = cell;
        }
        else
        {
            int newIndex = cells.Count;
            columnIndexMap[column] = newIndex;
            cells.Add(cell);
        }
    }
}