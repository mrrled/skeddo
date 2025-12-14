using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;

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
    IUnitOfWork unitOfWork) : ILessonServices
{
    public async Task<List<LessonDto>> GetLessonsByScheduleId(Guid scheduleId)
    {
        var lessonList = await lessonRepository.GetLessonsByScheduleIdAsync(scheduleId);
        return lessonList.ToLessonsDto();
    }

    public async Task<CreateLessonResult> AddLesson(CreateLessonDto lessonDto, Guid scheduleId)
    {
        if (lessonDto.SchoolSubject is null)
            throw new ArgumentException("Cannot create a lesson without a school subject"); //возможно стоит это вынести как-то в домен, больше похоже на бизнес-логику
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        if (schedule is null)
            throw new ArgumentException($"Cannot find a schedule with id {scheduleId}");
        var teacherDto = lessonDto.Teacher;
        var teacher = teacherDto is null ? null : await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        var classroom = lessonDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDto.Classroom.Id);
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id);
        var lessonNumber = lessonDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDto.LessonNumber.Number, lessonDto.LessonNumber.Time);
        var studyGroup = lessonDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id);
        var studySubgroup = lessonDto.StudySubgroup is null
            ? null
            : StudySubgroup.CreateStudySubgroup(studyGroup, lessonDto.StudySubgroup.Name);
        var lessonId = Guid.NewGuid();
        var draft = new LessonDraft(lessonId, lessonDto.ScheduleId, schoolSubject, lessonNumber, teacher, studyGroup, classroom,
            studySubgroup,
            lessonDto.Comment);
        var result = lessonFactory.CreateFromDraft(draft);
        if (result.IsSuccess)
        {
            var editedLessons = schedule.AddLesson(result.Value);
            await lessonRepository.AddAsync(result.Value, scheduleId);
            await lessonRepository.UpdateRangeAsync(editedLessons);
            await unitOfWork.SaveChangesAsync();
            return CreateLessonResult.Success(result.Value.ToLessonDto());
        }
        else
        {
            await lessonDraftRepository.AddAsync(draft, scheduleId);
            await unitOfWork.SaveChangesAsync();
            return CreateLessonResult.Downgraded(draft.ToLessonDraftDto());
        }

        
    }

    public async Task<EditLessonResult> EditLesson(LessonDto lessonDto, Guid scheduleId)
    {
        var schedule = await scheduleRepository.GetScheduleByIdAsync(scheduleId);
        var lesson = schedule.Lessons.FirstOrDefault(x => x.Id == lessonDto.Id);
        if (lesson is null)
            throw new ArgumentException($"Cannot find a lesson with id {lessonDto.Id}");
        var schoolSubject = lessonDto.SchoolSubject is null
            ? null
            : await schoolSubjectRepository.GetSchoolSubjectByIdAsync(lessonDto.SchoolSubject.Id);
        var lessonNumber = lessonDto.LessonNumber is null
            ? null
            : LessonNumber.CreateLessonNumber(lessonDto.LessonNumber.Number, lessonDto.LessonNumber.Time);
        var teacher = lessonDto.Teacher is null
            ? null
            : await teacherRepository.GetTeacherByIdAsync(lessonDto.Teacher.Id);
        var studyGroup = lessonDto.StudyGroup is null
            ? null
            : await studyGroupRepository.GetStudyGroupByIdAsync(lessonDto.StudyGroup.Id);
        var classroom = lessonDto.Classroom is null
            ? null
            : await classroomRepository.GetClassroomByIdAsync(lessonDto.Classroom.Id);
        if (lessonDto.SchoolSubject is null || lessonDto.LessonNumber is null || lessonDto.Teacher is null ||
            lessonDto.Classroom is null || lessonDto.StudyGroup is null)
        {
            var draft = LessonDraft.CreateFromLesson(lesson);
            draft.SetSchoolSubject(schoolSubject);
            draft.SetLessonNumber(lessonNumber);
            draft.SetTeacher(teacher);
            draft.SetStudyGroup(studyGroup);
            draft.SetClassroom(classroom);
            draft.SetComment(lessonDto.Comment);
            await lessonDraftRepository.AddAsync(draft, schedule.Id);
            await lessonRepository.Delete(lesson);
            await unitOfWork.SaveChangesAsync();
            return EditLessonResult.Downgraded(draft.ToLessonDraftDto());
        }

        var editedLesson = schedule.EditLesson(
            lessonDto.Id,
            schoolSubject,
            lessonNumber,
            teacher,
            studyGroup,
            classroom,
            lessonDto.Comment
        );
        await lessonRepository.UpdateRangeAsync(editedLesson);
        await unitOfWork.SaveChangesAsync();
        return EditLessonResult.Success(lesson.ToLessonDto());
    }

    public async Task DeleteLesson(LessonDto lessonDto, Guid scheduleId)
    {
        var lesson = await lessonRepository.GetLessonByIdAsync(lessonDto.Id);
        await lessonRepository.Delete(lesson);
        await unitOfWork.SaveChangesAsync();
    }
}