using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;

namespace newUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AvaloniaList<DtoTeacher> teachers = new(); 
        private AvaloniaList<LessonCardViewModel> lessonCards = new();
        
        private IService service;
        public AvaloniaList<DtoTeacher> Teachers
        {
            get => teachers;
            set => SetProperty(ref teachers, value);
        }
        
        public AvaloniaList<LessonCardViewModel> LessonCards
        {
            get => lessonCards;
            set => SetProperty(ref lessonCards, value);
        }

        public ICommand LoadTeachersCommand { get; }
        public ICommand HideTeachersCommand { get; }
        
        public ICommand LoadLessonsCommand { get; }
        public ICommand HideLessonsCommand { get; }

        public MainViewModel(IService service)
        {
            this.service = service;
            HideLessonsCommand = new AsyncRelayCommand(HideLessons);
            LoadTeachersCommand = new AsyncRelayCommand(LoadTeachers);
            HideTeachersCommand = new AsyncRelayCommand(HideTeachers);
            LoadLessonsCommand = new AsyncRelayCommand(LoadLessons);
        }
        
        private Task HideTeachers()
        {
            Teachers.Clear();
            return Task.CompletedTask;
        }
        
        private Task LoadTeachers()
        {
            var fetchedItems =  service.FetchTeachersFromBackend();
            var newTeachersList = new AvaloniaList<DtoTeacher>(fetchedItems);
            Teachers = newTeachersList;
            return Task.CompletedTask;
        }
        
        private Task HideLessons()
        {
            LessonCards.Clear();
            return Task.CompletedTask;
        }
        
        private Task LoadLessons()
        {
            var fetchedLessons = service.FetchLessonsFromBackend();
            LessonCards.Clear();
    
            foreach (var dtoLesson in fetchedLessons)
            {
                var cardVm = new LessonCardViewModel(service) { Lesson = dtoLesson };
                LessonCards.Add(cardVm);
            }
    
            return Task.CompletedTask;
        }
    }
}