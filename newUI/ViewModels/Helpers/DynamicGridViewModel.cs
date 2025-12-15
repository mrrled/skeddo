using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace newUI.ViewModels.Helpers;

public abstract class DynamicGridViewModel<TCell, TColumn, TRow> : ViewModelBase
    where TRow : notnull
    where TColumn : notnull
    where TCell : ViewModelBase
{
    private ObservableCollection<TColumn> columns = [];
    private ObservableCollection<TableDataRow<TCell, TColumn, TRow>> rows = [];
    
    public ObservableCollection<TColumn> Columns
    {
        get => columns;
        set => SetProperty(ref columns, value);
    }

    public ObservableCollection<TableDataRow<TCell, TColumn, TRow>> Rows
    {
        get => rows;
        set => SetProperty(ref rows, value);
    }

    public void LoadDataFromBackend(List<(TRow RowHeader, Dictionary<TColumn, TCell?> CellData)> data)
    {
        var newColumns = ExtractColumns(data);
        var newRows = CreateRows(data, newColumns);
        
        Columns = new ObservableCollection<TColumn>(newColumns);
        Rows = new ObservableCollection<TableDataRow<TCell, TColumn, TRow>>(newRows);
    }
    
    private List<TColumn> ExtractColumns(List<(TRow RowHeader, Dictionary<TColumn, TCell?> CellData)> data)
    {
        var newcColumns = new HashSet<TColumn>();
        foreach (var (_, cellData) in data)
        {
            foreach (var column in cellData.Keys)
            {
                newcColumns.Add(column);
            }
        }
        return newcColumns.ToList();
    }
    
    private List<TableDataRow<TCell, TColumn, TRow>> CreateRows(
        List<(TRow RowHeader, Dictionary<TColumn, TCell?> CellData)> data,
        List<TColumn> newColumns)
    {
        var newRows = new List<TableDataRow<TCell, TColumn, TRow>>();
        
        foreach (var (rowHeader, cellData) in data)
        {
            var row = new TableDataRow<TCell, TColumn, TRow>(rowHeader, newColumns);
            
            foreach (var column in newColumns)
            {
                if (cellData.TryGetValue(column, out var cell) && cell != null)
                {
                    row.SetCell(column, cell);
                }
                else
                {
                    row.SetCell(column, CreateEmptyCell());
                }
            }
            
            newRows.Add(row);
        }
        
        return newRows;
    }
    
    protected abstract TCell CreateEmptyCell();
}