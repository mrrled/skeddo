using System.Threading.Tasks;
using System.Windows.Input;
using Application.UIModels;
using Avalonia.Collections;

namespace newUI.ViewModels
{
    // Наследуемся от нашего базового класса
    public class MainViewModel : ViewModelBase
    {
        // AvaloniaList автоматически уведомляет View при добавлении/удалении элементов.
        private AvaloniaList<Teacher> items = new();
        public AvaloniaList<Teacher> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
        public ICommand LoadItemsCommand { get; }

        public MainViewModel()
        {
            LoadItemsCommand = new AsyncRelayCommand(LoadItems);
        }

        private Task LoadItems()
        {
            var fetchedItems =  Application.ItemFetcher.FetchTeachersFromBackend();
            Items.Clear();
            Items.AddRange(fetchedItems);
            return Task.CompletedTask;
        }
    }
}