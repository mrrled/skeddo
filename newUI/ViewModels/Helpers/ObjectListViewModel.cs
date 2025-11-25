using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Avalonia.Collections;

namespace newUI.ViewModels.Helpers;

public class ObjectListViewModel<TObject> : ViewModelBase 
    where TObject : ViewModelBase, IObjectListItem<TObject>
{
    private AvaloniaList<TObject> objects = new();
    private TObject? selectedItem;
    private IService service;

    public ObjectListViewModel(IService service, Func<Task>? command = null)
    {
        this.service = service;
        var selectItemCommand1 = command != null 
            ? new RelayCommandAsync(command) 
            : null;
        ShowItemsCommand = new RelayCommandAsync(Show);
        HideItemsCommand = new RelayCommandAsync(Hide);
        LoadItemsCommand = new RelayCommandAsync(LoadObjects);
        SelectItemCommand = selectItemCommand1 ?? new RelayCommandAsync(OnItemSelected);
    }
    
    public TObject? SelectedItem
    {
        get => selectedItem;
        set => SetProperty(ref selectedItem, value);
    }

    public AvaloniaList<TObject> Objects
    {
        get => objects;
        set => SetProperty(ref objects, value);
    }
    
    public ICommand ShowItemsCommand { get; set; }
    public ICommand HideItemsCommand { get; set; }
    public ICommand LoadItemsCommand { get; set; }
    public ICommand SelectItemCommand { get; set; }
    
    public bool ShouldBeVisible { get; set; } = true;

    private Task Hide()
    {
        ShouldBeVisible = false;
        return Task.CompletedTask;
    }

    private Task Show()
    {
        LoadObjects();
        ShouldBeVisible = true;
        return Task.CompletedTask;
    }

    private Task LoadObjects()
    {
        objects = TObject.FetchFromBackend(service);
        return Task.CompletedTask;
    }
    
    private Task OnItemSelected()
    {
        if (SelectedItem != null)
        {
            ShouldBeVisible = false;
        }
        return Task.CompletedTask;
    }
}