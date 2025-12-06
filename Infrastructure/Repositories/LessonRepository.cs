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

    public async Task<Lesson> GetLessonByIdAsync(int id)
    {
        var lessonDbo = await context.Lessons
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (lessonDbo is null)
            throw new InvalidOperationException();
        return lessonDbo.ToLesson();
    }

    public async Task<List<Lesson>> GetLessonsByIdsAsync(List<int> lessonIds)
    {
        var lessons = await context.Lessons
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Where(x => lessonIds.Contains(x.Id))
            .ToListAsync();
        return lessons.ToLessons();
    }

    public async Task AddAsync(Lesson lesson, int scheduleId)
    {
        var lessonDbo = lesson.ToLessonDbo();
        var lessonNumber =
            await context.LessonNumbers.FirstOrDefaultAsync(x =>
                x.ScheduleId == scheduleId && x.Number == lesson.LessonNumber.Number);
        if (lessonNumber is null)
            throw new InvalidOperationException();
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new InvalidOperationException();
        lessonDbo.ScheduleId = scheduleId;
        lessonDbo.LessonNumberId = lessonNumber.Id;
        await context.Lessons.AddAsync(lessonDbo);
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        var lessonDbo = await context.Lessons
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .FirstOrDefaultAsync(x => x.Id == lesson.Id);
        if (lessonDbo is null)
            throw new InvalidOperationException();
        DboMapper.Mapper.Map(lesson, lessonDbo);
    }

    public async Task Delete(Lesson lesson)
    {
        await context.Lessons.Where(x => x.Id == lesson.Id).ExecuteDeleteAsync();
    }

    public async Task UpdateRangeAsync(List<Lesson> lessons)
    {
        var lessonIds = lessons.Select(x => x.Id).ToList();
        var lessonDbos = await context.Lessons
            .Where(x => lessonIds.Contains(x.Id))
            .ToListAsync();
        var dboDict = lessonDbos.ToDictionary(x => x.Id);
        foreach (var lesson in lessons)
        {
            if (dboDict.TryGetValue(lesson.Id, out var lessonDbo))
                DboMapper.Mapper.Map(lesson, lessonDbo);
        }
    }
}