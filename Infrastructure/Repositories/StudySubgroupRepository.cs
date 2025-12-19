using Domain.IRepositories;
using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudySubgroupRepository(ScheduleDbContext context) : IStudySubgroupRepository
{
    public async Task AddAsync(StudySubgroup studySubgroup, Guid studyGroupId)
    {
        var studySubgroupDbo = studySubgroup.ToStudySubgroupDbo();
        studySubgroupDbo.Id = Guid.NewGuid();
        studySubgroupDbo.StudyGroupId = studyGroupId;
        var studyGroupDbo = await context.StudyGroups
            .FirstOrDefaultAsync(x => x.Id == studyGroupId);
        if (studyGroupDbo is null)
            throw new InvalidOperationException($"Study group with id {studyGroupId} not found.");
        await context.StudySubgroups.AddAsync(studySubgroupDbo);
    }

    public async Task UpdateAsync(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, Guid studyGroupId)
    {
        var studySubgroupDbo = await context.StudySubgroups
            .FirstOrDefaultAsync(x => x.StudyGroupId == studyGroupId && x.Name == oldStudySubgroup.Name);
        if (studySubgroupDbo is null)
            throw new InvalidOperationException($"Study subgroup with name {oldStudySubgroup.Name} not found.");
        var newStudySubgroupDbo = newStudySubgroup.ToStudySubgroupDbo();
        newStudySubgroupDbo.StudyGroupId = studyGroupId;
        DboMapper.Mapper.Map(newStudySubgroupDbo, studySubgroupDbo);
    }

    public async Task Delete(StudySubgroup studySubgroup, Guid studyGroupId)
    {
        var dbo = await context.StudySubgroups
            .FirstAsync(x => x.StudyGroupId == studyGroupId && x.Name == studySubgroup.Name);
        context.StudySubgroups.Remove(dbo);
    }
}