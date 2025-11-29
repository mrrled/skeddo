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
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new NullReferenceException();
        schedule.LessonNumbers.Add(lessonNumberDbo);
    }

    public async Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonNumberDbo = schedule.LessonNumbers.FirstOrDefault(x => x.Number == oldLessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(newLessonNumber, lessonNumberDbo);
    }

    public async Task Delete(LessonNumber lessonNumber, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonNumberDbo = schedule.LessonNumbers.FirstOrDefault(x => x.Number == lessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new NullReferenceException();
        schedule.LessonNumbers.Remove(lessonNumberDbo);
    }
}