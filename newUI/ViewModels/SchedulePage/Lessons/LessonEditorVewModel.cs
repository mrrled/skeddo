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

    public bool IsEditMode => !isPureCreation;

    public AvaloniaList<TeacherDto> Teachers { get; } = new();
    public AvaloniaList<ClassroomDto> Classrooms { get; } = new();
    public AvaloniaList<StudyGroupDto> StudyGroups { get; } = new();
    public AvaloniaList<SchoolSubjectDto> Subjects { get; } = new();
    public AvaloniaList<LessonNumberDto> TimeSlots { get; } = new();

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

    public LessonEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.scheduleId = scheduleId;
        this.isPureCreation = true;
        this.HeaderText = "Добавление урока";
        this.SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        _ = LoadDataAsync();
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
        selectedSubject = lesson.SchoolSubject;
        selectedLessonNumber = lesson.LessonNumber;

        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
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
            .FetchStudyGroupsFromBackendAsync();
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
            SelectedStudyGroup = StudyGroups.FirstOrDefault(x => x.Id == selectedStudyGroup.Id);
        if (selectedSubject != null) SelectedSubject = Subjects.FirstOrDefault(x => x.Id == selectedSubject.Id);
        if (selectedLessonNumber != null)
            SelectedLessonNumber = TimeSlots.FirstOrDefault(x => x.Number == selectedLessonNumber.Number);
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        EditLessonResult result;

        if (editingLesson == null || editingLesson.Id == Guid.Empty)
        {
            var res = await scope.ServiceProvider.GetRequiredService<ILessonServices>().AddLesson(new CreateLessonDto
            {
                ScheduleId = scheduleId, Teacher = SelectedTeacher, Classroom = SelectedClassroom,
                StudyGroup = SelectedStudyGroup, SchoolSubject = SelectedSubject, LessonNumber = SelectedLessonNumber
            }, scheduleId);
            result = res.IsDraft
                ? EditLessonResult.Downgraded(res.LessonDraft!)
                : EditLessonResult.Success(res.Lesson!);
        }
        else
        {
            editingLesson.Teacher = SelectedTeacher;
            editingLesson.Classroom = SelectedClassroom;
            editingLesson.StudyGroup = SelectedStudyGroup;
            editingLesson.SchoolSubject = SelectedSubject;
            editingLesson.LessonNumber = SelectedLessonNumber;

            try
            {
                result = await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                    .EditLesson(editingLesson, scheduleId);
            }
            catch
            {
                var draft = new LessonDraftDto
                {
                    Id = editingLesson.Id, Teacher = SelectedTeacher, Classroom = SelectedClassroom,
                    StudyGroup = SelectedStudyGroup, SchoolSubject = SelectedSubject,
                    LessonNumber = SelectedLessonNumber
                };
                result = await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                    .EditDraftLesson(draft, scheduleId);
            }
        }

        LessonSaved?.Invoke(result);
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
                try
                {
                    await scope.ServiceProvider.GetRequiredService<ILessonServices>()
                        .DeleteLesson(editingLesson!, scheduleId);
                }
                catch
                {
                    await scope.ServiceProvider.GetRequiredService<ILessonDraftServices>()
                        .DeleteLessonDraft(new LessonDraftDto { Id = idToDelete }, scheduleId);
                }
            }

            LessonDeleted?.Invoke(idToDelete);
            Window?.Close();
        }
    }
}