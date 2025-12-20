using System;
using System.Linq;
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
    public event Action<EditLessonResult>? LessonSaved;
    public event Action<Guid>? LessonDeleted;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly LessonDto? editingLesson;
    private readonly Guid scheduleId;
    private readonly bool isPureCreation;
    private bool showSubgroups;

    public bool IsEditMode => !isPureCreation;
    public bool CanSeeSubgroups => StudySubgroups != null && StudySubgroups.Any();

    public bool ShowSubgroups
    {
        get => showSubgroups;
        set
        {
            if (SetProperty(ref showSubgroups, value))
            {
                if (value == false)
                {
                    SelectedStudySubgroup = null;
                }

                OnPropertyChanged(nameof(SelectedStudySubgroup));
            }
        }
    }

    public AvaloniaList<TeacherDto> Teachers { get; } = new();
    public AvaloniaList<ClassroomDto> Classrooms { get; } = new();
    public AvaloniaList<StudyGroupDto> StudyGroups { get; } = new();
    public AvaloniaList<StudySubgroupDto>? StudySubgroups { get; private set; } = new();
    public AvaloniaList<SchoolSubjectDto> Subjects { get; } = new();
    public AvaloniaList<LessonNumberDto> TimeSlots { get; } = new();

    private TeacherDto? selectedTeacher;
    private ClassroomDto? selectedClassroom;
    private StudyGroupDto? selectedStudyGroup;
    private StudySubgroupDto? selectedStudySubgroup = null;
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

    public TeacherDto? OriginalTeacher { get; private set; }
    public ClassroomDto? OriginalClassroom { get; private set; }
    public StudyGroupDto? OriginalStudyGroup { get; private set; }
    public SchoolSubjectDto? OriginalSubject { get; private set; }
    public LessonNumberDto? OriginalLessonNumber { get; private set; }

    public StudyGroupDto? SelectedStudyGroup
    {
        get => selectedStudyGroup;
        set
        {
            if (SetProperty(ref selectedStudyGroup, value))
            {
                StudySubgroups = value is null ? null : new AvaloniaList<StudySubgroupDto>(value.StudySubgroups);
                OnPropertyChanged(nameof(StudySubgroups));
                OnPropertyChanged(nameof(CanSeeSubgroups));
            }
        }
    }

    public StudySubgroupDto? SelectedStudySubgroup
    {
        get => selectedStudySubgroup;
        set => SetProperty(ref selectedStudySubgroup, value);
    }

    public SchoolSubjectDto? SelectedSubject
    {
        get => selectedSubject;
        set
        {
            if (SetProperty(ref selectedSubject, value)) (SaveCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public LessonNumberDto? SelectedLessonNumber
    {
        get => selectedLessonNumber;
        set => SetProperty(ref selectedLessonNumber, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearTeacherCommand { get; }
    public ICommand ClearClassroomCommand { get; }
    public ICommand ClearStudyGroupCommand { get; }
    public ICommand ClearSubjectCommand { get; }
    public ICommand ClearLessonNumberCommand { get; }

    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.scheduleId = scheduleId;
        isPureCreation = true;
        HeaderText = "Добавление урока";
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        _ = LoadDataAsync();
        ClearTeacherCommand = new RelayCommand(() => SelectedTeacher = null);
        ClearClassroomCommand = new RelayCommand(() => SelectedClassroom = null);
        ClearStudyGroupCommand = new RelayCommand(() => {
            SelectedStudyGroup = null;
            SelectedStudySubgroup = null;
            ShowSubgroups = false;
        });
        ClearSubjectCommand = new RelayCommand(() => SelectedSubject = null);
        ClearLessonNumberCommand = new RelayCommand(() => SelectedLessonNumber = null);
    }

    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, LessonDto lesson)
        : this(scopeFactory, windowManager, lesson.ScheduleId)
    {
        this.editingLesson = lesson;
        this.isPureCreation = false;
        this.HeaderText = (lesson.Id == Guid.Empty) ? "Добавление урока" : "Редактирование урока";

        selectedTeacher = lesson.Teacher;
        selectedClassroom = lesson.Classroom;
        selectedStudyGroup = lesson.StudyGroup;
        selectedStudySubgroup = lesson.StudySubgroup;
        selectedSubject = lesson.SchoolSubject;
        selectedLessonNumber = lesson.LessonNumber;
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);

        if (lesson.StudySubgroup != null)
        {
            showSubgroups = true;
        }
    }

    private bool CanSave() => SelectedSubject != null;

    private async Task LoadDataAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var teachers = await scope.ServiceProvider.GetRequiredService<ITeacherServices>()
            .FetchTeachersFromBackendAsync();
        var classrooms = await scope.ServiceProvider.GetRequiredService<IClassroomServices>()
            .FetchClassroomsFromBackendAsync();
        var groups = await scope.ServiceProvider.GetRequiredService<IStudyGroupServices>()
            .GetStudyGroupByScheduleId(scheduleId);
        var subjects = await scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>()
            .FetchSchoolSubjectsFromBackendAsync();
        var slots = await scope.ServiceProvider.GetRequiredService<ILessonNumberServices>()
            .GetLessonNumbersByScheduleId(scheduleId);

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Teachers.AddRange(teachers);
            Classrooms.AddRange(classrooms);
            StudyGroups.AddRange(groups);
            Subjects.AddRange(subjects);
            TimeSlots.AddRange(slots);
            SyncSelectedItems();
        });
    }

    private void SyncSelectedItems()
    {
        if (selectedTeacher != null) SelectedTeacher = Teachers.FirstOrDefault(x => x.Id == selectedTeacher.Id);
        if (selectedClassroom != null) SelectedClassroom = Classrooms.FirstOrDefault(x => x.Id == selectedClassroom.Id);
        if (selectedStudyGroup != null)
        {
            SelectedStudyGroup = StudyGroups.FirstOrDefault(x => x.Id == selectedStudyGroup.Id);
            if (selectedStudySubgroup != null && StudySubgroups != null)
            {
                SelectedStudySubgroup = StudySubgroups.FirstOrDefault(x =>
                    x?.Name == selectedStudySubgroup?.Name);

                if (SelectedStudySubgroup != null)
                {
                    ShowSubgroups = true;
                }
            }
        }

        if (selectedSubject != null) SelectedSubject = Subjects.FirstOrDefault(x => x.Id == selectedSubject.Id);
        if (selectedLessonNumber != null) SelectedLessonNumber = TimeSlots.FirstOrDefault(x => x.Number == selectedLessonNumber.Number);
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        EditLessonResult? result = null;

        if (isPureCreation || editingLesson == null || editingLesson.Id == Guid.Empty)
        {
            // СОЗДАНИЕ НОВОГО
            var createDto = new CreateLessonDto
            {
                ScheduleId = scheduleId,
                Teacher = SelectedTeacher,
                Classroom = SelectedClassroom,
                StudyGroup = SelectedStudyGroup,
                StudySubgroup = SelectedStudySubgroup,
                SchoolSubject = SelectedSubject,
                LessonNumber = SelectedLessonNumber,
                Comment = ""
            };

            var response = await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                .AddLesson(createDto, scheduleId);

            if (response != null && response.IsSuccess)
            {
                var res = response.Value;
                result = res.IsDraft
                    ? EditLessonResult.Downgraded(res.LessonDraft!)
                    : EditLessonResult.Success(res.Lesson!);
            }
            else if (response != null)
            {
                // Если ошибка (например, не выбрали Предмет)
                // Здесь можно вывести уведомление пользователю
                return;
            }
        }
        else
        {
            // РЕДАКТИРОВАНИЕ
            var lessonDto = new LessonDto
            {
                Id = editingLesson.Id,
                ScheduleId = scheduleId,
                Teacher = SelectedTeacher,
                Classroom = SelectedClassroom,
                StudyGroup = SelectedStudyGroup,
                StudySubgroup = SelectedStudySubgroup,
                SchoolSubject = SelectedSubject,
                LessonNumber = SelectedLessonNumber
            };

            var lessonResponse = await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                .EditLesson(lessonDto, scheduleId);

            if (lessonResponse != null && lessonResponse.IsSuccess)
            {
                result = lessonResponse.Value;
            }
            else
            {
                var draftDto = new LessonDraftDto
                {
                    Id = editingLesson.Id,
                    ScheduleId = scheduleId,
                    Teacher = SelectedTeacher,
                    Classroom = SelectedClassroom,
                    StudyGroup = SelectedStudyGroup,
                    StudySubgroup = SelectedStudySubgroup,
                    SchoolSubject = SelectedSubject,
                    LessonNumber = SelectedLessonNumber
                };

                var draftResponse = await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                    .EditDraftLesson(draftDto, scheduleId);

                if (draftResponse != null && draftResponse.IsSuccess)
                {
                    result = draftResponse.Value;
                }
            }
        }

        if (result != null)
        {
            LessonSaved?.Invoke(result);
        }

        Window?.Close();
    }

    private async Task DeleteAsync()
    {
        var confirm = new ConfirmDeleteViewModel("Удалить этот урок?");
        if (await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirm) == true)
        {
            Guid idToDelete = editingLesson?.Id ?? Guid.Empty;
            if (idToDelete != Guid.Empty)
            {
                using var scope = scopeFactory.CreateScope();
                var lessonDto = new LessonDto { Id = idToDelete, ScheduleId = scheduleId };
                var res = await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                    .DeleteLesson(lessonDto, scheduleId);

                if (res == null || !res.IsSuccess)
                {
                    await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                        .DeleteLessonDraft(new LessonDraftDto { Id = idToDelete, ScheduleId = scheduleId }, scheduleId);
                }
            }

            LessonDeleted?.Invoke(idToDelete);
            Window?.Close();
        }
    }
}