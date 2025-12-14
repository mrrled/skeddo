using Application.DtoExtensions;
using Application.DtoModels;
using Application.IServices;
using Domain;
using Domain.IRepositories;
using Domain.Models;

namespace Application.Services;

public class LessonDraftServices(ILessonDraftRepository lessonDraftRepository,
    ILessonRepository lessonRepository,
    IScheduleRepository scheduleRepository,
    ISchoolSubjectRepository schoolSubjectRepository,
    ITeacherRepository teacherRepository,
    IStudyGroupRepository studyGroupRepository,
    IClassroomRepository classroomRepository,
    ILessonFactory lessonFactory,
    IUnitOfWork unitOfWork) : ILessonDraftServices
{
    public async Task<List<LessonDraftDto>> GetLessonDraftsByScheduleId(Guid scheduleId)
    {
        var drafts = await lessonDraftRepository.GetLessonDraftsByScheduleId(scheduleId);
        return drafts.ToLessonsDraftDto();
    }

    public async Task<LessonDraftDto> GetLessonDraftById(Guid id)
    {
        var lessonDraft = await lessonDraftRepository.GetLessonDraftById(id);
        return lessonDraft.ToLessonDraftDto();
    }

    public async Task<EditLessonResult> EditDraftLesson(LessonDraftDto lessonDraftDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        var lessonDraft = schedule.LessonDrafts.FirstOrDefault(x => x.Id == lessonDraftDto.Id);
        if (lessonDraft is null)
            throw new ArgumentException($"Cannot find a lessonDraft with id {lessonDraftDto.Id}");
        var schoolSubject = lessonDraftDto.SchoolSubject is null
            ? null
            : await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDraftDto.SchoolSubject.Id);
        var lessonNumber = lessonDraftDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDraftDto.LessonNumber.Number, lessonDraftDto.LessonNumber.Time);
        var teacher = lessonDraftDto.Teacher is null
            ? null
            : await teacherRepository.GetTeacherByIdAsync(lessonDraftDto.Teacher.Id);
        var studyGroup = lessonDraftDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDraftDto.StudyGroup.Id);
        var classroom = lessonDraftDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDraftDto.Classroom.Id);
        if (lessonDraftDto.SchoolSubject is null || lessonDraftDto.LessonNumber is null || lessonDraftDto.Teacher is null ||
            lessonDraftDto.Classroom is null || lessonDraftDto.StudyGroup is null)
        {
            lessonDraft.SetSchoolSubject(schoolSubject);
            lessonDraft.SetLessonNumber(lessonNumber);
            lessonDraft.SetTeacher(teacher);
            lessonDraft.SetStudyGroup(studyGroup);
            lessonDraft.SetClassroom(classroom);
            lessonDraft.SetComment(lessonDraftDto.Comment);
            await lessonDraftRepository.Update(lessonDraft);
            await unitOfWork.SaveChangesAsync();
            return EditLessonResult.Downgraded(lessonDraft.ToLessonDraftDto());
        }

        var result = lessonFactory.CreateFromDraft(lessonDraft);
        if (result.IsFailure)
            throw new Exception(result.Error);  //надо нормальный exception выкидывать
        var lesson = result.Value;
        await lessonRepository.AddAsync(lesson, scheduleId);
        await lessonDraftRepository.Delete(lessonDraft);
        await unitOfWork.SaveChangesAsync();
        return EditLessonResult.Success(lesson.ToLessonDto());
    }

    public async Task DeleteLessonDraft(LessonDraftDto lessonDto, Guid scheduleId)
    {
        var lesson = await lessonDraftRepository.GetLessonDraftById(lessonDto.Id);
        await lessonDraftRepository.Delete(lesson);
        await unitOfWork.SaveChangesAsync();
    }
}