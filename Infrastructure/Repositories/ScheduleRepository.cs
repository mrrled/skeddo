using Domain;
using Domain.Models;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ScheduleDbContext context, IMapper mapper) : IScheduleRepository
{
    public List<Teacher> GetTeachers()
    {
        var teachers = context.Teachers
            .Where(x => x.ScheduleGroupId == 1)
            .Include(teacherDbo => teacherDbo.SchoolSubjects)
            .Include(teacherDbo => teacherDbo.StudyGroups)
            .ToList();

        var result = teachers.ToTeacher(mapper);
        return teachers.ToTeacher(mapper);
    }

    public List<Lesson> GetLessonsByScheduleId(int scheduleId)
    {
        var lessons = context.Lessons.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .ToList();
        var result = lessons.ToLesson(mapper);
        return result;
    }

    public void AddTeacher(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public void AddLesson(Lesson lesson)
    {
        throw new NotImplementedException();
    }

    public void AddClassroom(Classroom classroom)
    {
        throw new NotImplementedException();
    }

    public void AddStudyGroup(StudyGroup studyGroup)
    {
        throw new NotImplementedException();
    }
}