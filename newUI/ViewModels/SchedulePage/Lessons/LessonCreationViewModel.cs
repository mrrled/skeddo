using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCreationViewModel : ViewModelBase
{
    public event Action<LessonDto>? LessonCreated;
    
    private readonly LessonDto lesson;
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
    
    public readonly int ScheduleId;
    
    public LessonCreationViewModel(IServiceScopeFactory scopeFactory,
        int scheduleId)
    {
        this.scopeFactory = scopeFactory;
        ScheduleId = scheduleId;
        lesson = new LessonDto();
        _ = Initialize();
        
        CreateLessonCommand = new AsyncRelayCommand(CreateLessonAsync);
    }

    private async Task Initialize()
    {
        await LoadDataAsync();
    }
    
    public ICommand LoadDataCommand { get; }
    public ICommand CreateLessonCommand { get; }
    public ICommand CancelCommand { get; }

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
                lesson.Teacher = value;
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
                lesson.Classroom = value;
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
                lesson.StudyGroup = value;
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
                lesson.SchoolSubject = value;
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
                lesson.LessonNumber = value;
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
    
    public IEnumerable<TeacherDto> FilterTeachers(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return Teachers;
        
        return Teachers.Where(t => 
            t.Name.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
            t.Surname.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
            t.Patronymic.Contains(searchText, System.StringComparison.OrdinalIgnoreCase));
    }
    
    public IEnumerable<ClassroomDto> FilterClassrooms(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return Classrooms;
        
        return Classrooms.Where(c => 
            c.Name.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) ||
            c.Description?.Contains(searchText, System.StringComparison.OrdinalIgnoreCase) == true);
    }
}