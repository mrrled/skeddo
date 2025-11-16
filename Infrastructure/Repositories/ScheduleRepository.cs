using AutoMapper;
using Domain;
using Domain.Models;
using Infrastructure.DboModels;
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
        var scheduleGroupId = 1;
        var dbo = teacher.ToTeacherDbo(mapper);
        context.Teachers.Add(dbo);
        var scheduleGroup = context.ScheduleGroups.FirstOrDefault(x => x.Id == 1);
        if (scheduleGroup is null)
            throw new ArgumentException($"Schedule group with {scheduleGroupId} id not found");
        scheduleGroup.Teachers.Add(dbo);
        context.SaveChanges();
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

    public void Update(Lesson lesson)
    {
        var lessonDbo = context.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        mapper.Map(lesson, lessonDbo);
        //_unitOfWork.SaveChangesAsync() в Application
    }

    public void UpdateClassroom(Lesson lesson, Classroom classroom)
    {
        var lessonDbo = context.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        if (lessonDbo is null)
            throw new ArgumentException($"Lesson id {lesson.Id} not found");
        lessonDbo.Classroom = mapper.Map<ClassroomDbo>(classroom);
        //_unitOfWork.SaveChangesAsync() в Application
    }
}