using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class LessonServices(
    IScheduleRepository scheduleRepository,
    ILessonRepository lessonRepository,
    ITeacherRepository teacherRepository,
    ISchoolSubjectRepository schoolSubjectRepository,
    IStudyGroupRepository studyGroupRepository,
    IClassroomRepository classroomRepository,
    ILessonFactory lessonFactory,
    ILessonDraftRepository lessonDraftRepository,
    IUnitOfWork unitOfWork,
    ILogger<LessonServices> logger) : BaseService(unitOfWork, logger), ILessonServices
{
    public async Task<List<LessonDto>> GetLessonsByScheduleId(Guid scheduleId)
    {
        var lessonList = await lessonRepository.GetLessonsByScheduleIdAsync(scheduleId);
        return lessonList.ToLessonsDto();
    }

    public async Task<Result<CreateLessonResult>> AddLesson(CreateLessonDto lessonDto, Guid scheduleId)
    {
        if (lessonDto.SchoolSubject is null)
            return Result<CreateLessonResult>.Failure("Нельзя создать урок без предмета.");
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            return Result<CreateLessonResult>.Failure("Расписание не найдено.");
        var teacherDto = lessonDto.Teacher;
        var teacher = teacherDto is null ? null : await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        var classroom = lessonDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDto.Classroom.Id);
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id);
        if (schoolSubject is null)
            return Result<CreateLessonResult>.Failure("Предмет не найден.");
        var lessonNumberCreateResult = lessonDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDto.LessonNumber.Number, lessonDto.LessonNumber.Time);
        if (lessonNumberCreateResult is not null && lessonNumberCreateResult.IsFailure)
            return Result<CreateLessonResult>.Failure(lessonNumberCreateResult.Error);
        var studyGroup = lessonDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id);
        var studySubgroupCreateResult = lessonDto.StudySubgroup is null || studyGroup is null
            ? null
            : StudySubgroup.CreateStudySubgroup(studyGroup, lessonDto.StudySubgroup.Name);
        if (studySubgroupCreateResult is not null && studySubgroupCreateResult.IsFailure)
            return Result<CreateLessonResult>.Failure(studySubgroupCreateResult.Error);
        var lessonId = Guid.NewGuid();
        var draft = new LessonDraft(lessonId,
            lessonDto.ScheduleId,
            schoolSubject,
            lessonNumberCreateResult?.Value,
            teacher,
            studyGroup,
            classroom,
            studySubgroupCreateResult?.Value,
            lessonDto.Comment);
        var result = lessonFactory.CreateFromDraft(draft);
        if (result.IsSuccess)
        {
            var editedLessons = schedule.AddLesson(result.Value);
            await lessonRepository.AddAsync(result.Value, scheduleId);
            await lessonRepository.UpdateRangeAsync(editedLessons);
            return await TrySaveChangesAsync(CreateLessonResult.Success(result.Value.ToLessonDto()),
                "Не удалось сохранить урок. Попробуйте позже.");
        }

        var addResult = await ExecuteRepositoryTask(() => lessonDraftRepository.AddAsync(draft, scheduleId),
            "Ошибка при добавлении урока. Попробуйте позже.");
        if (addResult.IsFailure)
            return Result<CreateLessonResult>.Failure(addResult.Error);
        return await TrySaveChangesAsync(CreateLessonResult.Downgraded(draft.ToLessonDraftDto()),
            "Не удалось сохранить урок. Попробуйте позже.");
    }

    public async Task<Result<EditLessonResult>> EditLesson(LessonDto lessonDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            return Result<EditLessonResult>.Failure("Расписание не найдено.");
        var lesson = schedule.Lessons.FirstOrDefault(x => x.Id == lessonDto.Id);
        if (lesson is null)
            return Result<EditLessonResult>.Failure("Урок не найден.");
        var schoolSubject = lessonDto.SchoolSubject is null
            ? null
            : await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id);
        var lessonNumberCreateResult = lessonDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDto.LessonNumber.Number, lessonDto.LessonNumber.Time);
        if (lessonNumberCreateResult is not null && lessonNumberCreateResult.IsFailure)
            return Result<EditLessonResult>.Failure(lessonNumberCreateResult.Error);
        var teacher = lessonDto.Teacher is null
            ? null
            : await teacherRepository.GetTeacherByIdAsync(lessonDto.Teacher.Id);
        var studyGroup = lessonDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id);
        var studySubgroupCreateResult = lessonDto.StudySubgroup is null || studyGroup is null
            ? null
            : StudySubgroup.CreateStudySubgroup(studyGroup, lessonDto.StudySubgroup.Name);
        if (studySubgroupCreateResult is not null && studySubgroupCreateResult.IsFailure)
            return Result<EditLessonResult>.Failure(studySubgroupCreateResult.Error);
        var classroom = lessonDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDto.Classroom.Id);
        if (lessonDto.SchoolSubject is null || lessonDto.LessonNumber is null || lessonDto.StudyGroup is null)
        {
            var draft = LessonDraft.CreateFromLesson(lesson);
            draft.SetSchoolSubject(schoolSubject);
            draft.SetLessonNumber(lessonNumberCreateResult?.Value);
            draft.SetTeacher(teacher);
            draft.SetStudyGroup(studyGroup);
            draft.SetClassroom(classroom);
            draft.SetComment(lessonDto.Comment);
            draft.SetStudySubgroup(studySubgroupCreateResult?.Value);
            var addResult = await ExecuteRepositoryTask(() => lessonDraftRepository.AddAsync(draft, schedule.Id),
                "Ошибка при добавлении урока. Попробуйте позже.");
            if (addResult.IsFailure)
                return Result<EditLessonResult>.Failure(addResult.Error);
            await lessonRepository.Delete(lesson);
            return await TrySaveChangesAsync(EditLessonResult.Downgraded(draft.ToLessonDraftDto()),
                "Не удалось изменить урок. Попробуйте позже.");
        }

        var editedLessonResult = schedule.EditLesson(
            lessonDto.Id,
            schoolSubject,
            lessonNumberCreateResult?.Value,
            teacher,
            studyGroup,
            classroom,
            studySubgroupCreateResult?.Value,
            lessonDto.Comment
        );
        if (editedLessonResult.IsFailure)
            return Result<EditLessonResult>.Failure(editedLessonResult.Error);
        var updateResult = await ExecuteRepositoryTask(
            () => lessonRepository.UpdateRangeAsync(editedLessonResult.Value),
            "Ошибка при изменении урока. Попробуйте позже.");
        if (updateResult.IsFailure)
            return Result<EditLessonResult>.Failure(updateResult.Error);
        return await TrySaveChangesAsync(EditLessonResult.Success(lesson.ToLessonDto()),
            "Не удалось изменить урок. Попробуйте позже.");
    }

    public async Task<Result> DeleteLesson(LessonDto lessonDto, Guid scheduleId)
    {
        var lesson = await lessonRepository.GetLessonByIdAsync(lessonDto.Id);
        if (lesson is null)
            return Result.Failure("Урок не найден.");
        await lessonRepository.Delete(lesson);
        return await TrySaveChangesAsync("Не удалось удалить урок. Попробуйте позже.");
    }
}