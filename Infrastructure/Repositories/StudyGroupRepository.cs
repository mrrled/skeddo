using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudyGroupRepository(ScheduleDbContext context) : IStudyGroupRepository
{
    public async Task<List<StudyGroup>> GetStudyGroupListAsync()
    {
        var studyGroupDbos = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .ToListAsync();
        return studyGroupDbos.ToStudyGroups();
    }

    public async Task<List<StudyGroup>> GetStudyGroupListByScheduleIdAsync(Guid scheduleId)
    {
        var studyGroupDbos = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .Where(x => x.ScheduleId == scheduleId)
            .ToListAsync();
        return studyGroupDbos.ToStudyGroups();
    }

    public async Task<StudyGroup?> GetStudyGroupByIdAsync(Guid studyGroupId)
    {
        var studyGroupDbo = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .FirstOrDefaultAsync(s => s.Id == studyGroupId);
        return studyGroupDbo?.ToStudyGroup();
    }

    public async Task<List<StudyGroup>> GetStudyGroupListByIdsAsync(List<Guid> studyGroupIds)
    {
        var studyGroupDbos = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .Where(x => studyGroupIds.Contains(x.Id))
            .ToListAsync();
        return studyGroupDbos.ToStudyGroups();
    }

    public async Task AddAsync(StudyGroup studyGroup, Guid scheduleId)
    {
        var studyGroupDbo = studyGroup.ToStudyGroupDbo();
        var scheduleGroup = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new InvalidOperationException($"Schedule with id {scheduleId} not found.");
        studyGroupDbo.ScheduleId = scheduleId;
        await context.StudyGroups.AddAsync(studyGroupDbo);
    }

    public async Task UpdateAsync(StudyGroup studyGroup)
    {
        var studyGroupDbo = await context.StudyGroups.FirstOrDefaultAsync(x => x.Id == studyGroup.Id);
        if (studyGroupDbo is null)
            throw new InvalidOperationException($"Study group with id {studyGroup.Id} not found.");
        DboMapper.Mapper.Map(studyGroup, studyGroupDbo);
    }

    public async Task Delete(StudyGroup studyGroup)
    {
        var studyGroupDbo = await context.StudyGroups
            .Include(s => s.StudySubgroups)
            .FirstAsync(x => x.Id == studyGroup.Id);
        context.StudyGroups.Remove(studyGroupDbo);
    }
}