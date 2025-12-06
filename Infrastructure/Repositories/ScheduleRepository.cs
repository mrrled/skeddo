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
    
    public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
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
        if (scheduleDbo is null)
            throw new InvalidOperationException();
        return scheduleDbo.ToSchedule();
    }
    
    //Для рабочести временно поменяла методы, пожалуйста не злитесь
    //Оригинальные методы в целости и сохранности
    
    // public async Task<List<Schedule>> GetScheduleListAsync()
    // {
    //     var scheduleGroup = await context.ScheduleGroups
    //         .Include(x => x.Schedules)
    //         .FirstOrDefaultAsync();
    //     if (scheduleGroup is null)
    //         throw new NullReferenceException();
    //     return scheduleGroup.Schedules.ToSchedules();
    // }

    // public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
    // {
    //     var scheduleGroup = await context.ScheduleGroups
    //         .Include(x => x.Schedules)
    //         .ThenInclude(schedule => schedule.Lessons)
    //         .FirstOrDefaultAsync();
    //     if (scheduleGroup is null)
    //         throw new NullReferenceException();
    //     var schedule = scheduleGroup.Schedules.FirstOrDefault(x => x.Id == scheduleId);
    //     if (schedule is null)
    //         throw new NullReferenceException();
    //     return schedule.ToSchedule();
    // }
    
    public async Task AddAsync(Schedule schedule, int scheduleGroupId)
    {
        var scheduleDbo = schedule.ToScheduleDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync(x => x.Id == scheduleGroupId);
        if (scheduleGroup is null)
            throw new InvalidOperationException();
        scheduleDbo.ScheduleGroupId = scheduleGroupId;
        await context.AddAsync(scheduleDbo);
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        var scheduleDbo = await context.Schedules
            .FirstOrDefaultAsync(x => x.Id == schedule.Id);
        if (scheduleDbo is null)
            throw new InvalidOperationException();
        DboMapper.Mapper.Map(schedule, scheduleDbo);
    }

    public async Task Delete(Schedule schedule)
    {
        await context.Schedules.Where(x => x.Id == schedule.Id).ExecuteDeleteAsync();
    }
}