using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(ScheduleDbContext context) : ILessonRepository
{
    public async Task<List<Lesson>> GetLessonsByScheduleIdAsync(Guid scheduleId)
    {
        var lessons = await context.Lessons.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .ThenInclude(x => x.StudySubgroups)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Include(x => x.StudySubgroup)
            .ToListAsync();
        return lessons.ToLessons();
    }

    public async Task<Lesson?> GetLessonByIdAsync(Guid id)
    {
        var lessonDbo = await context.Lessons
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .ThenInclude(x => x.StudySubgroups)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Include(x => x.StudySubgroup)
            .FirstOrDefaultAsync(x => x.Id == id);
        return lessonDbo?.ToLesson();
    }

    public async Task<List<Lesson>> GetLessonsByIdsAsync(List<Guid> lessonIds)
    {
        var lessons = await context.Lessons
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .ThenInclude(x => x.StudySubgroups)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Include(x => x.StudySubgroup)
            .Where(x => lessonIds.Contains(x.Id))
            .ToListAsync();
        return lessons.ToLessons();
    }

    public async Task AddAsync(Lesson lesson, Guid scheduleId)
    {
        var lessonDbo = lesson.ToLessonDbo();
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new InvalidOperationException($"Schedule with id {scheduleId} not found.");
        var lessonNumber =
            await context.LessonNumbers.FirstOrDefaultAsync(x =>
                x.ScheduleId == scheduleId && x.Number == lesson.LessonNumber.Number);
        if (lessonNumber is null)
            throw new InvalidOperationException($"Lesson number with number {lesson.LessonNumber.Number} not found.");
        var studySubgroup = lesson.StudySubgroup is null
            ? null
            : await context.StudySubgroups.FirstOrDefaultAsync(x =>
                x.StudyGroup.Id == lesson.StudyGroup.Id && x.Name == lesson.StudySubgroup.Name);
        lessonDbo.StudySubgroupId = studySubgroup?.Id;
        lessonDbo.ScheduleId = scheduleId;
        lessonDbo.LessonNumberId = lessonNumber.Id;
        lessonDbo.WarningType = (int)lesson.WarningType;
        await context.Lessons.AddAsync(lessonDbo);
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        var lessonDbo = await context.Lessons
            .FirstOrDefaultAsync(x => x.Id == lesson.Id);

        if (lessonDbo is null)
            throw new InvalidOperationException($"Lesson with id {lesson.Id} not found.");

        DboMapper.Mapper.Map(lesson, lessonDbo);

        // Добавляем ручную привязку номера урока
        var lessonNumberDbo = await context.LessonNumbers
            .FirstOrDefaultAsync(x => x.ScheduleId == lesson.ScheduleId
                                      && x.Number == lesson.LessonNumber.Number);

        lessonDbo.LessonNumberId = lessonNumberDbo?.Id ?? lessonDbo.LessonNumberId;
    }

    public async Task Delete(Lesson lesson)
    {
        var lessonDbo = await context.Lessons.FirstAsync(x => x.Id == lesson.Id);
        context.Lessons.Remove(lessonDbo);
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
            if (!dboDict.TryGetValue(lesson.Id, out var lessonDbo))
                continue;
            DboMapper.Mapper.Map(lesson, lessonDbo);
            lessonDbo.WarningType = (int)lesson.WarningType;
            var lessonNumber =
                await context.LessonNumbers
                    .FirstOrDefaultAsync(x => x.Number == lesson.LessonNumber.Number
                                              && x.ScheduleId == lesson.ScheduleId);
            if (lessonNumber is null)
                throw new InvalidOperationException(
                    $"Lesson number with number {lesson.LessonNumber.Number} not found.");
            lessonDbo.LessonNumberId = lessonNumber.Id;
            if (lesson.StudySubgroup is null)
            {
                lessonDbo.StudySubgroupId = null;
                continue;
            }

            var studySubgroup = await context.StudySubgroups
                .FirstOrDefaultAsync(x => x.Name == lesson.StudySubgroup.Name
                                          && x.StudyGroup.Id == lesson.StudyGroup.Id);
            if (studySubgroup is null)
                throw new InvalidOperationException($"Study subgroup with name {lesson.StudySubgroup.Name} not found.");
            lessonDbo.StudySubgroupId = studySubgroup.Id;
        }
    }
}