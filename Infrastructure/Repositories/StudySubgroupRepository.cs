using Domain.IRepositories;
using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudySubgroupRepository(ScheduleDbContext context) : IStudySubgroupRepository
{
    public async Task AddAsync(StudySubgroup studySubgroup, int studyGroupId)
    {
        var random = new Random();
        var studySubgroupDbo = studySubgroup.ToStudySubgroupDbo();
        studySubgroupDbo.Id = random.Next(1, 1000);
        studySubgroupDbo.StudyGroupId = studyGroupId;
        var studyGroupDbo = await context.StudyGroups
            .FirstOrDefaultAsync(x => x.Id == studyGroupId);
        if (studyGroupDbo is null)
            throw new InvalidOperationException();
        await context.StudySubgroups.AddAsync(studySubgroupDbo);
    }

    public async Task UpdateAsync(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, int studyGroupId)
    {
        var studySubgroupDbo = await context.StudySubgroups
            .FirstOrDefaultAsync(x => x.StudyGroupId == studyGroupId && x.Name == oldStudySubgroup.Name);
        if (studySubgroupDbo is null)
            throw new InvalidOperationException();
        var newStudySubgroupDbo = newStudySubgroup.ToStudySubgroupDbo();
        newStudySubgroupDbo.StudyGroupId = studyGroupId;
        DboMapper.Mapper.Map(newStudySubgroupDbo, studySubgroupDbo);
    }

    public async Task Delete(StudySubgroup studySubgroup, int studyGroupId)
    {
        await context.StudySubgroups
            .Where(x => x.StudyGroupId == studyGroupId && x.Name == studySubgroup.Name)
            .ExecuteDeleteAsync();
    }
}