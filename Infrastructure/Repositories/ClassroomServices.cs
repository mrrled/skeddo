using Domain.Models;
using Domain.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClassroomRepository(ScheduleDbContext context) : IClassroomRepository
{
    public async Task<List<Classroom>> GetClassroomListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.Classrooms.ToClassroom();
    }
    
    public async Task AddAsync(Classroom classroom)
    {
        var classroomDbo = classroom.ToClassroomDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.Classrooms.Add(classroomDbo);
    }

    public async Task UpdateAsync(Classroom oldClassroom, Classroom newClassroom)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var classroomDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == oldClassroom.Name);
        if (classroomDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(newClassroom, classroomDbo);
    }

    public async Task Delete(Classroom classroom)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var classroomDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == classroom.Name);
        if (classroomDbo is null)
            throw new NullReferenceException();
        scheduleGroup.Classrooms.Remove(classroomDbo);
    }
}