using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class LessonServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : ILessonServices
{
    public async Task<List<LessonDto>> GetLessonsByScheduleId(int scheduleId)
    {
        var lessonList = await repository.GetLessonsByScheduleIdAsync(scheduleId);
        return lessonList.ToLessonDto();
    }
    
    public async Task AddLesson(LessonDto lessonDto, int scheduleId)
    {
        Teacher? teacher = null;
        var schedule = await repository.GetScheduleByIdAsync(scheduleId);
        var teacherDto = lessonDto.Teacher;
        if (teacherDto is not null)
            teacher = Schedule.CreateTeacher(
                teacherDto.Id,
                teacherDto.Name,
                teacherDto.Surname,
                teacherDto.Patronymic,
                teacherDto.SchoolSubjects,
                teacherDto.StudyGroups);
        var lesson = schedule.AddLesson(
            lessonDto.Id,
            lessonDto.Subject?.Name,
            lessonDto.LessonNumber.Number,
            teacher,
            lessonDto.StudyGroup?.Name,
            lessonDto.Classroom?.Name,
            lessonDto.Classroom?.Description
        );
        await repository.AddAsync(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLesson(LessonDto lessonDto, int scheduleId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonDto.Id, scheduleId);
        lesson.Update(
            lessonDto.Subject?.ToSchoolSubject(),
            lessonDto.LessonNumber?.ToLessonNumber(),
            lessonDto.Teacher?.ToTeacher(),
            lessonDto.StudyGroup?.ToStudyGroup(),
            lessonDto.Classroom?.ToClassroom(),
            lessonDto.Comment
        );
        await repository.UpdateAsync(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLesson(LessonDto lessonDto, int scheduleId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonDto.Id, scheduleId);
        await repository.Delete(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }
}