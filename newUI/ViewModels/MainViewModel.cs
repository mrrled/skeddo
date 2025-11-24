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
        private AvaloniaList<TeacherDto> items = new();
        private ITeacherServices teacherService;

        public AvaloniaList<TeacherDto> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public ICommand LoadItemsCommand { get; }
        public ICommand HideTeachersCommand { get; }

        public MainViewModel(ITeacherServices teacherService)
        {
            this.teacherService = teacherService;
            LoadItemsCommand = new RelayCommandAsync(LoadItems);
            HideTeachersCommand = new RelayCommandAsync(HideItems);
        }

        private Task HideItems()
        {
            Items.Clear();
            return Task.CompletedTask;
        }

        private async Task LoadItems()
        {
            var fetchedItems = await teacherService.FetchTeachersFromBackendAsync();
            Items.Clear();
            Items.AddRange(fetchedItems);
        }
    }
}