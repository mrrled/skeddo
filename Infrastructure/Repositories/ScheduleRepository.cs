using Domain.Models;
using Domain.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ScheduleDbContext context) : IScheduleRepository
{
    public async Task<List<Schedule>> GetScheduleListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.Schedules.ToSchedule();
    }

    public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schedule = scheduleGroup.Schedules.FirstOrDefault(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        return schedule.ToSchedule();
    }
    
    public async Task AddAsync(Schedule schedule)
    {
        var scheduleDbo = schedule.ToScheduleDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.Schedules.Add(scheduleDbo);
    }

    public async Task UpdateAsync(Schedule oldSchedule, Schedule newSchedule)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var scheduleDbo = scheduleGroup.Schedules.FirstOrDefault(x => x.Id == oldSchedule.Id);
        if (scheduleDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(newSchedule, scheduleDbo);
    }

    public async Task Delete(Schedule schedule)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var scheduleDbo = scheduleGroup.Schedules.FirstOrDefault(x => x.Id == schedule.Id);
        if (scheduleDbo is null)
            throw new NullReferenceException();
        scheduleGroup.Schedules.Remove(scheduleDbo);
    }
}