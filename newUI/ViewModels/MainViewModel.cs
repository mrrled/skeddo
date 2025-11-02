using System.Threading.Tasks;
using System.Windows.Input;
using Application;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;

namespace newUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AvaloniaList<DtoTeacher> items = new();
        private IService service;
        public AvaloniaList<DtoTeacher> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
        public ICommand LoadItemsCommand { get; }
        public ICommand HideTeachersCommand { get; }

        public MainViewModel(IService service)
        {
            this.service = service;
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
            var fetchedItems =  service.FetchTeachersFromBackend();
            Items.Clear();
            Items.AddRange(fetchedItems);
            return Task.CompletedTask;
        }
    }
}