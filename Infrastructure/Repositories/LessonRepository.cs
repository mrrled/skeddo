using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(ScheduleDbContext context) : ILessonRepository
{
    public async Task<List<Lesson>> GetLessonsByScheduleIdAsync(int scheduleId)
    {
        var lessons = await context.Lessons.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .ToListAsync();
        return lessons.ToLessons();
    }

    public async Task<Lesson> GetLessonByIdAsync(int id, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lesson = schedule.Lessons.FirstOrDefault(x => x.Id == id);
        if (lesson is null)
            throw new NullReferenceException();
        return lesson.ToLesson();
    }

    public async Task AddAsync(Lesson lesson, int scheduleId)
    {
        var lessonDbo = lesson.ToLessonDbo();
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new NullReferenceException();
        schedule.Lessons.Add(lessonDbo);
    }

    public async Task UpdateAsync(Lesson lesson, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.Lessons)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonDbo = schedule.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        if (lessonDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(lesson, lessonDbo);
    }

    public async Task Delete(Lesson lesson, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.Lessons)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonDbo = schedule.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        if (lessonDbo is null)
            throw new NullReferenceException();
        schedule.Lessons.Remove(lessonDbo);
    }
}