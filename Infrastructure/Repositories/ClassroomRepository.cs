using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClassroomRepository(ScheduleDbContext context) : IClassroomRepository
{
    public async Task<List<Classroom>> GetClassroomListAsync(int scheduleGroupId)
    {
        var classrooms = await context.Classrooms.Where(x => x.ScheduleGroupId == scheduleGroupId).ToListAsync();
        return classrooms.ToClassrooms();
    }

    public async Task<Classroom?> GetClassroomByIdAsync(Guid classroomId)
    {
        var scheduleGroup = await context.ScheduleGroups.Include(x => x.Classrooms).FirstOrDefaultAsync();
        var classroomDbo = scheduleGroup?.Classrooms.FirstOrDefault(x => x.Id == classroomId);
        return classroomDbo?.ToClassroom();
    }

    public async Task AddAsync(Classroom classroom, int scheduleGroupId)
    {
        var classroomDbo = classroom.ToClassroomDbo();
        var scheduleGroup = await context.ScheduleGroups
            .Where(x => x.Id == scheduleGroupId)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new InvalidOperationException($"Schedule group with Id {scheduleGroupId} not found.");
        classroomDbo.ScheduleGroupId = scheduleGroupId;
        await context.AddAsync(classroomDbo);
    }

    public async Task UpdateAsync(Classroom classroom)
    {
        var classroomDbo = await context.Classrooms.FirstOrDefaultAsync(x => x.Id == classroom.Id);
        if (classroomDbo is null)
            throw new InvalidOperationException($"Classroom with Id {classroom.Id} not found.");
        DboMapper.Mapper.Map(classroom, classroomDbo);
    }

    public async Task Delete(Classroom classroom)
    {
        var dbo = await context.Classrooms.FirstAsync(x => x.Id == classroom.Id);
        context.Classrooms.Remove(dbo);
    }
}