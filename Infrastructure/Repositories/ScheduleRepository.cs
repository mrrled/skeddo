using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ScheduleDbContext context) : IScheduleRepository
{
    public async Task<List<Schedule>> GetScheduleListAsync(int scheduleGroupId)
    {
        var scheduleDbos = await context.Schedules
            .Where(x => x.ScheduleGroupId == scheduleGroupId)
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
            .Include(s => s.Lessons)
            .ThenInclude(l => l.StudySubgroup)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.LessonNumber)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.StudyGroup)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.Teacher)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.SchoolSubject)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.Classroom)
            .ToListAsync();
        return scheduleDbos.ToSchedules();
    }

    public async Task<Schedule?> GetScheduleByIdAsync(Guid scheduleId)
    {
        var scheduleDbo = await context.Schedules
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
            .Include(s => s.Lessons)
            .ThenInclude(l => l.StudySubgroup)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.LessonNumber)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.StudyGroup)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.Teacher)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.SchoolSubject)
            .Include(s => s.LessonDrafts)
            .ThenInclude(l => l.Classroom)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);
        return scheduleDbo?.ToSchedule();
    }

    public async Task AddAsync(Schedule schedule, int scheduleGroupId)
    {
        var scheduleDbo = schedule.ToScheduleDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync(x => x.Id == scheduleGroupId);
        if (scheduleGroup is null)
            throw new InvalidOperationException($"Schedule group with id {scheduleGroupId} not found.");
        scheduleDbo.ScheduleGroupId = scheduleGroupId;
        await context.AddAsync(scheduleDbo);
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        var scheduleDbo = await context.Schedules
            .FirstOrDefaultAsync(x => x.Id == schedule.Id);
        if (scheduleDbo is null)
            throw new InvalidOperationException($"Schedule with id {schedule.Id} not found.");
        DboMapper.Mapper.Map(schedule, scheduleDbo);
    }

    public async Task Delete(Schedule schedule)
    {
        var sched = await context.Schedules
            .Include(s => s.Lessons)
            .Include(s => s.LessonDrafts)
            .Include(s => s.LessonNumbers)
            .FirstAsync(s => s.Id == schedule.Id);
        context.Schedules.Remove(sched);
    }
}