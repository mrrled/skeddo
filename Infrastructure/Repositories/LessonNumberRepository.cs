using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonNumberRepository(ScheduleDbContext context) : ILessonNumberRepository
{
    public async Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(Guid scheduleId)
    {
        var schedule = await context.Schedules
            .Include(x => x.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new InvalidOperationException($"Schedule with id {scheduleId} not found.");
        return schedule.LessonNumbers.ToLessonNumbers();
    }

    public async Task AddAsync(LessonNumber lessonNumber, Guid scheduleId)
    {
        var lessonNumberDbo = lessonNumber.ToLessonNumberDbo();
        lessonNumberDbo.ScheduleId = scheduleId;
        lessonNumberDbo.Id = Guid.NewGuid();
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new InvalidOperationException($"Schedule with id {scheduleId} not found.");
        await context.LessonNumbers.AddAsync(lessonNumberDbo);
    }

    public async Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, Guid scheduleId)
    {
        var lessonNumberDbo = await context.LessonNumbers
            .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId && x.Number == oldLessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new InvalidOperationException(
                $"Lesson number with number {oldLessonNumber.Number} and schedule id {scheduleId} not found.");
        var newLessonNumberDbo = newLessonNumber.ToLessonNumberDbo();
        newLessonNumberDbo.ScheduleId = scheduleId;
        DboMapper.Mapper.Map(newLessonNumberDbo, lessonNumberDbo);
    }

    public async Task Delete(LessonNumber lessonNumber, Guid scheduleId)
    {
        var dbo = await context.LessonNumbers
            .FirstAsync(x => x.ScheduleId == scheduleId && x.Number == lessonNumber.Number);
        context.LessonNumbers.Remove(dbo);
        var numbers = await context.LessonNumbers
            .Where(x => x.ScheduleId == scheduleId && x.Number != lessonNumber.Number).ToListAsync();
        var sorted = numbers.OrderBy(x => x.Number).ToList();
        for (var i = 1; i <= numbers.Count; i++)
            sorted[i - 1].Number = i;
    }
}