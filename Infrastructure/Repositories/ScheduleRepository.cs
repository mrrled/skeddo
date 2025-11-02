using AutoMapper;
using Domain;
using Domain.Models;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ScheduleDbContext context, IMapper mapper) : IScheduleRepository
{
    public List<Classroom> GetClassrooms()
    {
        throw new NotImplementedException();
    }

    public List<Schedule> GetSchedules()
    {
        throw new NotImplementedException();
    }

    public List<SchoolSubject> GetSchoolSubjects()
    {
        throw new NotImplementedException();
    }

    public List<StudyGroup> GetStudyGroups()
    {
        throw new NotImplementedException();
    }

    public List<Teacher> GetTeachers()
    {
        var teachers = context.Teachers
            .Where(x => x.ScheduleGroupId == 1)
            .Include(teacherDbo => teacherDbo.SchoolSubjects)
            .Include(teacherDbo => teacherDbo.StudyGroups)
            .ToList();
        
        return teachers.ToTeacher(mapper);
    }

    public List<TimeSlot> GetTimeSlots()
    {
        throw new NotImplementedException();
    }

    public List<Lesson> GetLessonsByScheduleId(int scheduleId)
    {
        var lessons = context.Lessons.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .ToList();
        return lessons.ToLesson(mapper);
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