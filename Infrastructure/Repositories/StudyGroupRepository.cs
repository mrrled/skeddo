using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudyGroupRepository(ScheduleDbContext context) : IStudyGroupRepository
{
    public async Task<List<StudyGroup>> GetStudyGroupListAsync(int scheduleGroupId)
    {
        var studyGroupDbos = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .Where(x => x.ScheduleGroupId == scheduleGroupId)
            .ToListAsync();
        return studyGroupDbos.ToStudyGroups();
    }

    public async Task<StudyGroup> GetStudyGroupByIdAsync(int studyGroupId)
    {
        var studyGroupDbo = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .FirstOrDefaultAsync(s => s.Id == studyGroupId);
        if (studyGroupDbo is null)
            throw new InvalidOperationException();
        return studyGroupDbo.ToStudyGroup();
    }

    public async Task<List<StudyGroup>> GetStudyGroupListByIdsAsync(List<int> studyGroupIds)
    {
        var studyGroupDbos = await context.StudyGroups
            .Include(x => x.StudySubgroups)
            .Where(x => studyGroupIds.Contains(x.Id))
            .ToListAsync();
        return studyGroupDbos.ToStudyGroups();
    }

    public async Task AddAsync(StudyGroup studyGroup, int scheduleGroupId)
    {
        var studyGroupDbo = studyGroup.ToStudyGroupDbo();
        var scheduleGroup = await context.ScheduleGroups
            .Where(x => x.Id == scheduleGroupId)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new InvalidOperationException();
        studyGroupDbo.ScheduleGroupId = scheduleGroupId;
        await context.StudyGroups.AddAsync(studyGroupDbo);
    }

    public async Task UpdateAsync(StudyGroup studyGroup)
    {
        var studyGroupDbo = await context.StudyGroups.FirstOrDefaultAsync(x => x.Id == studyGroup.Id);
        if (studyGroupDbo is null)
            throw new InvalidOperationException();
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