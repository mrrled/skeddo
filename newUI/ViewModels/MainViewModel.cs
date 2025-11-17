using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.Services;
using newUI.ViewModels.Lessons;

namespace newUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AvaloniaList<DtoTeacher> teachers = new(); 
        private AvaloniaList<string> columnHeaders = new();
        private AvaloniaList<LessonCardViewModel> lessonCards = new();
        private AvaloniaList<ScheduleRowViewModel> scheduleRows = new();
        
        private IService service;
        private IWindowManager windowManager;
        
        public double Width { get; set; }
        
        public AvaloniaList<DtoTeacher> Teachers
        {
            get => teachers;
            set => SetProperty(ref teachers, value);
        }
        
        public AvaloniaList<string> ColumnHeaders
        {
            get => columnHeaders;
            set => SetProperty(ref columnHeaders, value);
        }
        
        public AvaloniaList<ScheduleRowViewModel> ScheduleRows
        {
            get => scheduleRows;
            set => SetProperty(ref scheduleRows, value);
        }
        
        public AvaloniaList<LessonCardViewModel> LessonCards
        {
            get => lessonCards;
            set => SetProperty(ref lessonCards, value);
        }
        
        
        public ICommand CreateTeacherCommand {get; }
        public ICommand LoadTeachersCommand { get; }
        public ICommand HideTeachersCommand { get; }
        
        public ICommand LoadLessonsCommand { get; }
        public ICommand HideLessonsCommand { get; }

        public MainViewModel(IService service, IWindowManager windowManager)
        {
            this.service = service;
            this.windowManager = windowManager;
            CreateTeacherCommand = new AsyncRelayCommand(CreateTeacher);
            HideLessonsCommand = new AsyncRelayCommand(HideLessons);
            LoadTeachersCommand = new AsyncRelayCommand(LoadTeachers);
            HideTeachersCommand = new AsyncRelayCommand(HideTeachers);
            LoadLessonsCommand = new AsyncRelayCommand(LoadLessons);
        }

        private Task CreateTeacher()
        {
            windowManager.Show<TeacherCreationViewModel>();
            return Task.CompletedTask;
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
            ScheduleRows.Clear();
            ColumnHeaders.Clear();
            return Task.CompletedTask;
        }
        
        private Task LoadLessons()
        {
            var fetchedLessons = service.FetchLessonsFromBackend();
            
            var orderedStudyGroups = new AvaloniaList<string>(fetchedLessons
                .Select(l => l.StudyGroup.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToList());
            
            ColumnHeaders = orderedStudyGroups;
            
            var orderedTimeSlots = fetchedLessons
                .Select(l => new 
                { 
                    Order = l.TimeSlot.Number
                })
                .Distinct()
                .OrderBy(ts => ts.Order)
                .ToList();
            
            var lessonsByTimeSlot = fetchedLessons.GroupBy(l => l.TimeSlot.Number.ToString());

            var newScheduleRows = new List<ScheduleRowViewModel>();
            var maxWidth = 0.0;
//
            foreach (var timeSlot in orderedTimeSlots)
            {
                var rowVm = new ScheduleRowViewModel
                {
                    TimeSlotOrder = timeSlot.Order
                };
                
                var lessonsInSlot = lessonsByTimeSlot
                    .FirstOrDefault(g => g.Key == timeSlot.Order.ToString())?
                    .ToDictionary(l => l.StudyGroup.Name);
                
                foreach (var groupName in orderedStudyGroups)
                {
                    if (lessonsInSlot != null && lessonsInSlot.TryGetValue(groupName, out var lessonDto))
                    {
                        var lessonView = new LessonCardViewModel(service) { Lesson = lessonDto };
                        rowVm.Cells.Add(lessonView);
                        maxWidth = maxWidth < lessonView.Width ? lessonView.Width : maxWidth;
                    }
                    else
                        rowVm.Cells.Add(null);
                }

                newScheduleRows.Add(rowVm);
            }

            Width = maxWidth;
            ScheduleRows = new AvaloniaList<ScheduleRowViewModel>(newScheduleRows);
            
            return Task.CompletedTask;
        }
    }
}