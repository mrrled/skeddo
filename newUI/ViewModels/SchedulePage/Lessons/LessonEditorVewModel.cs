using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<EditLessonResult>? LessonSaved; // Универсальное событие сохранения
    public event Action<LessonDto>? LessonDeleted;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly LessonDto? editingLesson;
    private readonly Guid scheduleId;

    public bool IsEditMode => editingLesson != null;

    // Списки данных
    public AvaloniaList<TeacherDto> Teachers { get; } = new();
    public AvaloniaList<ClassroomDto> Classrooms { get; } = new();
    public AvaloniaList<StudyGroupDto> StudyGroups { get; } = new();
    public AvaloniaList<SchoolSubjectDto> Subjects { get; } = new();
    public AvaloniaList<LessonNumberDto> TimeSlots { get; } = new();

    // Поля ввода
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

    // Конструктор СОЗДАНИЯ
    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.scheduleId = scheduleId;
        this.HeaderText = "Добавление урока";

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        _ = LoadDataAsync();
    }

    // Конструктор РЕДАКТИРОВАНИЯ
    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, LessonDto lesson)
        : this(scopeFactory, windowManager, lesson.ScheduleId)
    {
        this.editingLesson = lesson;
        this.HeaderText = "Редактирование урока";

        SelectedTeacher = lesson.Teacher;
        SelectedClassroom = lesson.Classroom;
        SelectedStudyGroup = lesson.StudyGroup;
        SelectedSubject = lesson.SchoolSubject;
        SelectedLessonNumber = lesson.LessonNumber;

        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        EditLessonResult finalResult;

        if (!IsEditMode)
        {
            // СОЗДАНИЕ
            var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            var createDto = new CreateLessonDto
            {
                ScheduleId = scheduleId,
                Teacher = SelectedTeacher,
                Classroom = SelectedClassroom,
                StudyGroup = SelectedStudyGroup,
                SchoolSubject = SelectedSubject,
                LessonNumber = SelectedLessonNumber
            };

            var createResult = await service.AddLesson(createDto, scheduleId);

            // Используем статические методы модели для создания результата
            finalResult = createResult.IsDraft
                ? EditLessonResult.Downgraded(createResult.LessonDraft!)
                : EditLessonResult.Success(createResult.Lesson!);
        }
        else
        {
            // РЕДАКТИРОВАНИЕ
            editingLesson!.Teacher = SelectedTeacher;
            editingLesson.Classroom = SelectedClassroom;
            editingLesson.StudyGroup = SelectedStudyGroup;
            editingLesson.SchoolSubject = SelectedSubject;
            editingLesson.LessonNumber = SelectedLessonNumber;

            try
            {
                var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
                finalResult = await service.EditLesson(editingLesson, scheduleId);
            }
            catch (ArgumentException)
            {
                var draftService = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();
                var draftDto = new LessonDraftDto
                {
                    Id = editingLesson.Id,
                    Teacher = SelectedTeacher,
                    Classroom = SelectedClassroom,
                    StudyGroup = SelectedStudyGroup,
                    SchoolSubject = SelectedSubject,
                    LessonNumber = SelectedLessonNumber
                };
                finalResult = await draftService.EditDraftLesson(draftDto, scheduleId);
            }
        }

        LessonSaved?.Invoke(finalResult);
        Window?.Close();
    }

    private async Task DeleteAsync()
    {
        var confirmVm = new ConfirmDeleteViewModel("Вы уверены, что хотите удалить этот урок?");
        var dialogResult = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);
        if (dialogResult != true) return;

        using var scope = scopeFactory.CreateScope();
        try
        {
            await scope.ServiceProvider.GetRequiredService<ILessonServices>().DeleteLesson(editingLesson!, scheduleId);
        }
        catch
        {
            await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                .DeleteLessonDraft(new LessonDraftDto { Id = editingLesson!.Id }, scheduleId);
        }

        LessonDeleted?.Invoke(editingLesson!);
        Window?.Close();
    }

    private async Task LoadDataAsync()
    {
        using var scope = scopeFactory.CreateScope();
        Teachers.AddRange(await scope.ServiceProvider.GetRequiredService<ITeacherServices>()
            .FetchTeachersFromBackendAsync());
        Classrooms.AddRange(await scope.ServiceProvider.GetRequiredService<IClassroomServices>()
            .FetchClassroomsFromBackendAsync());
        StudyGroups.AddRange(await scope.ServiceProvider.GetRequiredService<IStudyGroupServices>()
            .FetchStudyGroupsFromBackendAsync());
        Subjects.AddRange(await scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>()
            .FetchSchoolSubjectsFromBackendAsync());
        TimeSlots.AddRange(await scope.ServiceProvider.GetRequiredService<ILessonNumberServices>()
            .GetLessonNumbersByScheduleId(scheduleId));
    }
}