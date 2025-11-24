using Domain.Models;
using Domain.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudyGroupRepository(ScheduleDbContext context) : IStudyGroupRepository
{
    public async Task<List<StudyGroup>> GetStudyGroupListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.StudyGroups.ToStudyGroup();
    }
    
    public async Task AddAsync(StudyGroup studyGroup)
    {
        var studyGroupDbo = studyGroup.ToStudyGroupDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.StudyGroups.Add(studyGroupDbo);
    }

    public async Task UpdateAsync(StudyGroup oldStudyGroup, StudyGroup newStudyGroup)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var studyGroupDbo = scheduleGroup.StudyGroups.FirstOrDefault(x => x.Name == oldStudyGroup.Name);
        if (studyGroupDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(newStudyGroup, studyGroupDbo);
    }

    public async Task Delete(StudyGroup studyGroup)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var studyGroupDbo = scheduleGroup.StudyGroups.FirstOrDefault(x => x.Name == studyGroup.Name);
        if (studyGroupDbo is null)
            throw new NullReferenceException();
        scheduleGroup.StudyGroups.Remove(studyGroupDbo);
    }
}