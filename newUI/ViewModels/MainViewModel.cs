using System.Threading.Tasks;
using System.Windows.Input;
using Application;
using Application.UIModels;
using Avalonia.Collections;

namespace newUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AvaloniaList<Teacher> items = new();
        public AvaloniaList<Teacher> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
        public ICommand LoadItemsCommand { get; }
        public ICommand HideTeachersCommand { get; }

        public MainViewModel()
        {
            LoadItemsCommand = new AsyncRelayCommand(LoadItems);
            HideTeachersCommand = new AsyncRelayCommand(HideItems);
        }

        private Task HideItems()
        {
            Items.Clear();
            return Task.CompletedTask;
        }

        private Task LoadItems()
        {
            var fetchedItems =  ItemFetcher.FetchTeachersFromBackend();
            Items.Clear();
            Items.AddRange(fetchedItems);
            return Task.CompletedTask;
        }
    }
}