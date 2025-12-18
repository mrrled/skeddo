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
    public event Action<EditLessonResult>? LessonResultUpdated;
    public event Action<CreateLessonDto>? LessonCreated;

    private readonly LessonDto originalLessonDto;
    private readonly CreateLessonDto lessonCreationDto;
    private readonly IServiceScopeFactory scopeFactory;

    public Guid ScheduleId { get; }
    public bool IsCreation { get; }

    // Списки для комбобоксов
    public AvaloniaList<TeacherDto> Teachers { get; private set; } = new();
    public AvaloniaList<ClassroomDto> Classrooms { get; private set; } = new();
    public AvaloniaList<StudyGroupDto> StudyGroups { get; private set; } = new();
    public AvaloniaList<SchoolSubjectDto> Subjects { get; private set; } = new();
    public AvaloniaList<LessonNumberDto> TimeSlots { get; private set; } = new();

    // Выбранные элементы
    private TeacherDto? selectedTeacher;
    private ClassroomDto? selectedClassroom;
    private StudyGroupDto? selectedStudyGroup;
    private SchoolSubjectDto? selectedSubject;
    private LessonNumberDto? selectedLessonNumber;

    public TeacherDto? SelectedTeacher
    {
        get => selectedTeacher;
        set => SetProperty(ref selectedTeacher, value);
    }

    public ClassroomDto? SelectedClassroom
    {
        get => selectedClassroom;
        set => SetProperty(ref selectedClassroom, value);
    }

    public StudyGroupDto? SelectedStudyGroup
    {
        get => selectedStudyGroup;
        set => SetProperty(ref selectedStudyGroup, value);
    }

    public SchoolSubjectDto? SelectedSubject
    {
        get => selectedSubject;
        set => SetProperty(ref selectedSubject, value);
    }

    public LessonNumberDto? SelectedLessonNumber
    {
        get => selectedLessonNumber;
        set => SetProperty(ref selectedLessonNumber, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    // Конструктор для создания
    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, Guid scheduleId)
    {
        this.IsCreation = true;
        this.scopeFactory = scopeFactory;
        this.ScheduleId = scheduleId;
        this.originalLessonDto = new LessonDto { Id = Guid.Empty };
        this.lessonCreationDto = new CreateLessonDto { ScheduleId = scheduleId };

        SaveCommand = new AsyncRelayCommand(OnSaveClicked);
        DeleteCommand = new AsyncRelayCommand(DeleteLessonAsync);
        CancelCommand = new RelayCommand(() => Window?.Close());
        _ = LoadDataAsync();
    }

    // Конструктор для редактирования
    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, LessonDto lessonDTO)
    {
        this.IsCreation = false;
        this.scopeFactory = scopeFactory;
        this.ScheduleId = lessonDTO.ScheduleId;
        this.originalLessonDto = lessonDTO;

        SelectedTeacher = lessonDTO.Teacher;
        SelectedClassroom = lessonDTO.Classroom;
        SelectedStudyGroup = lessonDTO.StudyGroup;
        SelectedSubject = lessonDTO.SchoolSubject;
        SelectedLessonNumber = lessonDTO.LessonNumber;

        SaveCommand = new AsyncRelayCommand(OnSaveClicked);
        DeleteCommand = new AsyncRelayCommand(DeleteLessonAsync);
        CancelCommand = new RelayCommand(() => Window?.Close());
        _ = LoadDataAsync();
    }

    private async Task OnSaveClicked()
    {
        if (IsCreation)
        {
            lessonCreationDto.Teacher = SelectedTeacher;
            lessonCreationDto.Classroom = SelectedClassroom;
            lessonCreationDto.StudyGroup = SelectedStudyGroup;
            lessonCreationDto.SchoolSubject = SelectedSubject;
            lessonCreationDto.LessonNumber = SelectedLessonNumber;
            LessonCreated?.Invoke(lessonCreationDto);
        }
        else
        {
            await SaveExistingAsync();
        }

        Window?.Close();
    }

    private async Task SaveExistingAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var lessonService = scope.ServiceProvider.GetRequiredService<ILessonServices>();
        var draftService = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();

        originalLessonDto.Teacher = SelectedTeacher;
        originalLessonDto.Classroom = SelectedClassroom;
        originalLessonDto.StudyGroup = SelectedStudyGroup;
        originalLessonDto.SchoolSubject = SelectedSubject;
        originalLessonDto.LessonNumber = SelectedLessonNumber;

        EditLessonResult result;
        try
        {
            // Пытаемся сохранить как полноценный урок
            result = await lessonService.EditLesson(originalLessonDto, ScheduleId);
        }
        catch (ArgumentException)
        {
            // Если не найден в уроках — значит это черновик из буфера
            var draftDto = new LessonDraftDto
            {
                Id = originalLessonDto.Id,
                SchoolSubject = originalLessonDto.SchoolSubject,
                Teacher = originalLessonDto.Teacher,
                Classroom = originalLessonDto.Classroom,
                LessonNumber = originalLessonDto.LessonNumber,
                StudyGroup = originalLessonDto.StudyGroup,
                Comment = originalLessonDto.Comment
            };
            result = await draftService.EditDraftLesson(draftDto, ScheduleId);
        }

        LessonResultUpdated?.Invoke(result);
    }

    private async Task LoadDataAsync()
    {
        using var scope = scopeFactory.CreateScope();
        Teachers = new AvaloniaList<TeacherDto>(await scope.ServiceProvider.GetRequiredService<ITeacherServices>()
            .FetchTeachersFromBackendAsync());
        Classrooms = new AvaloniaList<ClassroomDto>(await scope.ServiceProvider.GetRequiredService<IClassroomServices>()
            .FetchClassroomsFromBackendAsync());
        StudyGroups = new AvaloniaList<StudyGroupDto>(await scope.ServiceProvider
            .GetRequiredService<IStudyGroupServices>().FetchStudyGroupsFromBackendAsync());
        Subjects = new AvaloniaList<SchoolSubjectDto>(await scope.ServiceProvider
            .GetRequiredService<ISchoolSubjectServices>().FetchSchoolSubjectsFromBackendAsync());
        TimeSlots = new AvaloniaList<LessonNumberDto>(await scope.ServiceProvider
            .GetRequiredService<ILessonNumberServices>().GetLessonNumbersByScheduleId(ScheduleId));

        OnPropertyChanged(nameof(Teachers));
        OnPropertyChanged(nameof(Classrooms));
        OnPropertyChanged(nameof(StudyGroups));
        OnPropertyChanged(nameof(Subjects));
        OnPropertyChanged(nameof(TimeSlots));
    }

    private async Task DeleteLessonAsync()
    {
        using var scope = scopeFactory.CreateScope();
        try
        {
            await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                .DeleteLesson(originalLessonDto, ScheduleId);
        }
        catch
        {
            var draftDto = new LessonDraftDto { Id = originalLessonDto.Id };
            await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                .DeleteLessonDraft(draftDto, ScheduleId);
        }

        Window?.Close();
    }
}