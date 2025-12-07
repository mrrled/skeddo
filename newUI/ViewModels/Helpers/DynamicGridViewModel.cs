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
    private ObservableCollection<TColumn> columns = new();
    private ObservableCollection<TableDataRow<TCell, TColumn, TRow>> rows = [];

    public ObservableCollection<TableDataRow<TCell, TColumn, TRow>> Rows
    {
        get => rows;
        set => SetProperty(ref rows, value);
    }
    public ObservableCollection<TColumn> Columns { get; } = [];

    public void LoadDataFromBackend(Func<List<(TRow RowHeader, Dictionary<TColumn, TCell> CellData)>> dataLoader)
    {
        var rawRows = dataLoader();
        
        Rows.Clear();
        Columns.Clear();
        
        if (rawRows == null || rawRows.Count == 0) return;

        foreach (var rowData in rawRows)
        {
            var newRow = new TableDataRow<TCell, TColumn, TRow>(rowData.RowHeader);
            
            foreach (var cellKvp in rowData.CellData)
            {
                newRow.SetCell(cellKvp.Key, cellKvp.Value);
            }
            Rows.Add(newRow);
        }

        var allColumns = rawRows
            .SelectMany(r => r.CellData.Keys)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        if (allColumns.First() is IComparable)
        {
            allColumns = allColumns.OrderBy(c => c).ToList();
        }

        foreach (var column in allColumns)
        {
            Columns.Add(column);
        }
    }
    
    public void AddRow(TRow rowHeader)
    {
        var newRow = new TableDataRow<TCell, TColumn, TRow>(rowHeader);
        Rows.Add(newRow);
    }
    
    public void UpdateCell(TRow rowHeader, TColumn columnHeader, TCell value)
    {
        var row = Rows.FirstOrDefault(r => r.RowHeader.Equals(rowHeader));
        if (row != null)
        {
            row[columnHeader] = value;
        }
    }
}