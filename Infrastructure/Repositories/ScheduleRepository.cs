using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Infrastructure.DboModels;
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
        return scheduleGroup.Schedules.ToSchedules();
    }
    
    public async Task<List<Schedule>> GetScheduleListWithLessonsAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        
        if (scheduleGroup is null)
            throw new NullReferenceException();
        
        var hui = await context.Schedules
            .Where(s => s.ScheduleGroupId == scheduleGroup.Id)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.LessonNumber)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.StudyGroup)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.Teacher)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.SchoolSubject)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.Classroom)
            .Include(s => s.LessonNumbers)
            .ToListAsync();
        return hui.ToSchedules();
    }

    public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var hui = await context.Schedules
            .Where(s => s.ScheduleGroupId == scheduleGroup.Id)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.LessonNumber)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.StudyGroup)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.Teacher)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.SchoolSubject)
            .Include(s => s.Lessons)
            .ThenInclude(l => l.Classroom)
            .Include(s => s.LessonNumbers)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);
        return hui.ToSchedule();
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