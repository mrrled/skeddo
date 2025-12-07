using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonNumberRepository(ScheduleDbContext context) : ILessonNumberRepository
{
    public async Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(x => x.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        return schedule.LessonNumbers.ToLessonNumbers();
    }

    public async Task AddAsync(LessonNumber lessonNumber, int scheduleId)
    {
        var lessonNumberDbo = lessonNumber.ToLessonNumberDbo();
        lessonNumberDbo.ScheduleId = scheduleId;
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new NullReferenceException();
        await context.LessonNumbers.AddAsync(lessonNumberDbo);
    }

    public async Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, int scheduleId)
    {
        var lessonNumberDbo = await context.LessonNumbers
            .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId && x.Number == oldLessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new InvalidOperationException();
        var newLessonNumberDbo = newLessonNumber.ToLessonNumberDbo();
        newLessonNumberDbo.ScheduleId = scheduleId;
        DboMapper.Mapper.Map(newLessonNumberDbo, lessonNumberDbo);
    }

    public async Task Delete(LessonNumber lessonNumber, int scheduleId)
    {
        await context.LessonNumbers
            .Where(x => x.ScheduleId == scheduleId && x.Number == lessonNumber.Number)
            .ExecuteDeleteAsync();
    }
}