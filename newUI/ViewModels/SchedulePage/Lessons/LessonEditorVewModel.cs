using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonEditorViewModel : ViewModelBase
{
    public event Action<LessonDto>? LessonUpdated;
    public event Action<CreateLessonDto>? LessonCreated;
    
    private readonly LessonDto originalLesson;
    private readonly CreateLessonDto lesson;
    private readonly IServiceScopeFactory scopeFactory;

    private AvaloniaList<TeacherDto> teachers;
    private AvaloniaList<ClassroomDto> classrooms;
    private AvaloniaList<StudyGroupDto> studyGroups;
    private AvaloniaList<SchoolSubjectDto> subjects;
    private AvaloniaList<LessonNumberDto> timeSlots;
    
    private TeacherDto? selectedTeacher;
    private ClassroomDto? selectedClassroom;
    private StudyGroupDto? selectedStudyGroup;
    private SchoolSubjectDto? selectedSubject;
    private LessonNumberDto? selectedTimeSlot;
    
    public readonly Guid ScheduleId;

    public LessonEditorViewModel(
        IServiceScopeFactory scopeFactory,
        Guid scheduleId)
    {
        IsCreation = true;
        this.scopeFactory = scopeFactory;
        originalLesson = new LessonDto();
        ScheduleId = scheduleId;
        lesson = new CreateLessonDto();
        
        _ = Initialize();
        
        SaveCommand = new AsyncRelayCommand(CreateLessonAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteLessonAsync);
        CancelCommand = new RelayCommand(() => Window?.Close());
    }
    
    public LessonEditorViewModel(
        IServiceScopeFactory scopeFactory,
        LessonDto lesson)
    {
        IsCreation = false;
        this.scopeFactory = scopeFactory;
        originalLesson = lesson;
        ScheduleId = lesson.ScheduleId;
        
        selectedTeacher = lesson.Teacher;
        selectedClassroom = lesson.Classroom;
        selectedStudyGroup = lesson.StudyGroup;
        selectedSubject = lesson.SchoolSubject;
        selectedTimeSlot = lesson.LessonNumber;
        
        _ = Initialize();
        
        SaveCommand = new AsyncRelayCommand(SaveLessonAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteLessonAsync);
        CancelCommand = new RelayCommand(() => Window?.Close());
    }

    private async Task Initialize()
    {
        await LoadDataAsync();
    }
    
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }
    
    public bool IsCreation { get; }

    public AvaloniaList<TeacherDto> Teachers
    {
        get => teachers;
        set => SetProperty(ref teachers, value);
    }
    
    public AvaloniaList<ClassroomDto> Classrooms
    {
        get => classrooms;
        set => SetProperty(ref classrooms, value);
    }
    
    public AvaloniaList<StudyGroupDto> StudyGroups
    {
        get => studyGroups;
        set => SetProperty(ref studyGroups, value);
    }
    
    public AvaloniaList<SchoolSubjectDto> Subjects
    {
        get => subjects;
        set => SetProperty(ref subjects, value);
    }
    
    public AvaloniaList<LessonNumberDto> TimeSlots
    {
        get => timeSlots;
        set => SetProperty(ref timeSlots, value);
    }
    
    public TeacherDto SelectedTeacher
    {
        get => selectedTeacher;
        set
        {
            if (SetProperty(ref selectedTeacher, value))
            {
                if (IsCreation)
                {
                    lesson.Teacher = value;
                }
                originalLesson.Teacher = value;
            }
        }
    }
    
    public ClassroomDto SelectedClassroom
    {
        get => selectedClassroom;
        set
        {
            if (SetProperty(ref selectedClassroom, value))
            {
                if (IsCreation)
                {
                    lesson.Classroom = value;
                }
                originalLesson.Classroom = value;
            }
        }
    }
    
    public StudyGroupDto SelectedStudyGroup
    {
        get => selectedStudyGroup;
        set
        {
            if (SetProperty(ref selectedStudyGroup, value))
            {
                if (IsCreation)
                {
                    lesson.StudyGroup = value;
                }
                originalLesson.StudyGroup = value;
            }
        }
    }
    
    public SchoolSubjectDto SelectedSubject
    {
        get => selectedSubject;
        set
        {
            if (SetProperty(ref selectedSubject, value))
            {
                if (IsCreation)
                {
                    lesson.SchoolSubject = value;
                }
                originalLesson.SchoolSubject = value;
            }
        }
    }
    
    public LessonNumberDto SelectedTimeSlot
    {
        get => selectedTimeSlot;
        set
        {
            if (SetProperty(ref selectedTimeSlot, value))
            {
                if (IsCreation)
                {
                    lesson.LessonNumber = value;
                }
                originalLesson.LessonNumber = value;
            }
        }
    }
    
    private async Task LoadDataAsync()
    {
        using var scope = scopeFactory.CreateScope();
        
        var teacherService = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
        var teachers = await teacherService.FetchTeachersFromBackendAsync();
        Teachers = new AvaloniaList<TeacherDto>(teachers);
        
        var classroomService = scope.ServiceProvider.GetRequiredService<IClassroomServices>();
        var classrooms = await classroomService.FetchClassroomsFromBackendAsync();
        Classrooms = new AvaloniaList<ClassroomDto>(classrooms);
        
        var studyGroupService = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();
        var studyGroups = await studyGroupService.FetchStudyGroupsFromBackendAsync();
        StudyGroups = new AvaloniaList<StudyGroupDto>(studyGroups);
        
        var subjectService = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();
        var subjects = await subjectService.FetchSchoolSubjectsFromBackendAsync();
        Subjects = new AvaloniaList<SchoolSubjectDto>(subjects);
        
        var timeSlotService = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();
        var lessonNumbers = await timeSlotService.GetLessonNumbersByScheduleId(ScheduleId);
        TimeSlots = new AvaloniaList<LessonNumberDto>(lessonNumbers);
    }
    
    private async Task CreateLessonAsync()
    {
        using var scope = scopeFactory.CreateScope();
        LessonCreated?.Invoke(lesson);
        Console.WriteLine(Window);
        Window?.Close();
    }
    
    private async Task SaveLessonAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
        
        originalLesson.Teacher = selectedTeacher;
        originalLesson.Classroom = selectedClassroom;
        originalLesson.StudyGroup = selectedStudyGroup;
        originalLesson.SchoolSubject = selectedSubject;
        originalLesson.LessonNumber = selectedTimeSlot;
        
        await service.EditLesson(originalLesson, ScheduleId);
        LessonUpdated?.Invoke(originalLesson);
        Window?.Close();
    }
    
    private async Task DeleteLessonAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
        
        await service.DeleteLesson(originalLesson, ScheduleId);
        Window?.Close();
    }
}