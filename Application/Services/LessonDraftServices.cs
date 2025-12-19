using Application.DtoExtensions;
using Application.DtoModels;
using Application.IServices;
using Domain;
using Domain.IRepositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class LessonDraftServices(
    ILessonDraftRepository lessonDraftRepository,
    ILessonRepository lessonRepository,
    IScheduleRepository scheduleRepository,
    ISchoolSubjectRepository schoolSubjectRepository,
    ITeacherRepository teacherRepository,
    IStudyGroupRepository studyGroupRepository,
    IClassroomRepository classroomRepository,
    ILessonFactory lessonFactory,
    IUnitOfWork unitOfWork,
    ILogger<LessonDraftServices> logger) : BaseService(unitOfWork, logger), ILessonDraftServices
{
    public async Task<List<LessonDraftDto>> GetLessonDraftsByScheduleId(Guid scheduleId)
    {
        var drafts = await lessonDraftRepository.GetLessonDraftsByScheduleId(scheduleId);
        return drafts.ToLessonsDraftDto();
    }

    public async Task<Result<LessonDraftDto>> GetLessonDraftById(Guid id)
    {
        var lessonDraft = await lessonDraftRepository.GetLessonDraftById(id);
        if (lessonDraft is null)
            return Result<LessonDraftDto>.Failure("Урок не найден.");
        return Result<LessonDraftDto>.Success(lessonDraft.ToLessonDraftDto());
    }

    public async Task<Result<EditLessonResult>> EditDraftLesson(LessonDraftDto lessonDraftDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            return Result<EditLessonResult>.Failure("Расписание не найдено.");
        var lessonDraft = schedule.LessonDrafts.FirstOrDefault(x => x.Id == lessonDraftDto.Id);
        if (lessonDraft is null)
            return Result<EditLessonResult>.Failure("Урок не найден.");
        var schoolSubject = lessonDraftDto.SchoolSubject is null
            ? null
            : await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id);
        var lessonNumberCreateResult = lessonDraftDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDraftDto.LessonNumber.Number, lessonDraftDto.LessonNumber.Time);
        if (lessonNumberCreateResult is not null && lessonNumberCreateResult.IsFailure)
            return Result<EditLessonResult>.Failure(lessonNumberCreateResult.Error);
        var teacher = lessonDraftDto.Teacher is null
            ? null
            : await teacherRepository.GetTeacherByIdAsync(lessonDraftDto.Teacher.Id);
        var studyGroup = lessonDraftDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDraftDto.StudyGroup.Id);
        var studySubgroupCreateResult = lessonDraftDto.StudySubgroup is null || studyGroup is null
            ? null
            : StudySubgroup.CreateStudySubgroup(studyGroup, lessonDraftDto.StudySubgroup.Name);
        if (studySubgroupCreateResult is not null && studySubgroupCreateResult.IsFailure)
            return Result<EditLessonResult>.Failure(studySubgroupCreateResult.Error);
        var classroom = lessonDraftDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDraftDto.Classroom.Id);
        if (lessonDraftDto.SchoolSubject is null || lessonDraftDto.LessonNumber is null ||
            lessonDraftDto.Teacher is null ||
            lessonDraftDto.Classroom is null || lessonDraftDto.StudyGroup is null)
        {
            lessonDraft.SetStudySubgroup(studySubgroupCreateResult?.Value);
            lessonDraft.SetSchoolSubject(schoolSubject);
            lessonDraft.SetLessonNumber(lessonNumberCreateResult?.Value);
            lessonDraft.SetTeacher(teacher);
            lessonDraft.SetStudyGroup(studyGroup);
            lessonDraft.SetClassroom(classroom);
            lessonDraft.SetComment(lessonDraftDto.Comment);
            var updateResult = await ExecuteRepositoryTask(() => lessonDraftRepository.Update(lessonDraft),
                "Ошибка при изменении урока. Попробуйте позже.");
            if (updateResult.IsFailure)
                return Result<EditLessonResult>.Failure(updateResult.Error);
            return await TrySaveChangesAsync(EditLessonResult.Downgraded(lessonDraft.ToLessonDraftDto()),
                "Не удалось изменить урок. Попробуйте позже.");
        }

        var result = lessonFactory.CreateFromDraft(lessonDraft);
        if (result.IsFailure)
            return Result<EditLessonResult>.Failure(result.Error);
        var lesson = result.Value;
        var addResult = await ExecuteRepositoryTask(() => lessonRepository.AddAsync(lesson, scheduleId),
            "Ошибка при изменении урока. Попробуйте позже");
        if (addResult.IsFailure)
            return Result<EditLessonResult>.Failure(addResult.Error);
        await lessonDraftRepository.Delete(lessonDraft);
        return await TrySaveChangesAsync(EditLessonResult.Success(lesson.ToLessonDto()),
            "Не удалось изменить урок. Попробуйте позже.");
    }

    public async Task<Result> DeleteLessonDraft(LessonDraftDto lessonDto, Guid scheduleId)
    {
        var lesson = await lessonDraftRepository.GetLessonDraftById(lessonDto.Id);
        if (lesson is null)
            return Result.Failure("Урок не найден.");
        await lessonDraftRepository.Delete(lesson);
        return await TrySaveChangesAsync("Не удалось удалить урок. Попробуйте позже.");
    }

    public async Task ClearDraftsByScheduleId(Guid scheduleId)
    {
        var drafts = await lessonDraftRepository.GetLessonDraftsByScheduleId(scheduleId);
        foreach (var draft in drafts)
        {
            await lessonDraftRepository.Delete(draft);
        }

        await unitOfWork.SaveChangesAsync();
    }
}