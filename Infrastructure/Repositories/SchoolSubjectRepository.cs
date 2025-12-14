using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SchoolSubjectRepository(ScheduleDbContext context) : ISchoolSubjectRepository
{
    public async Task<List<SchoolSubject>> GetSchoolSubjectListAsync(int scheduleGroupId)
    {
        var schoolSubjectDbos = await context.SchoolSubjects
            .Where(x => x.ScheduleGroupId == scheduleGroupId)
            .ToListAsync();
        return schoolSubjectDbos.ToSchoolSubjects();
    }

    public async Task<SchoolSubject> GetSchoolSubjectByIdAsync(Guid schoolSubjectId)
    {
        var schoolSubjectDbo = await context.SchoolSubjects.FirstOrDefaultAsync(s => s.Id == schoolSubjectId);
        if (schoolSubjectDbo is null)
            throw new InvalidOperationException();
        return schoolSubjectDbo.ToSchoolSubject();
    }

    public async Task<List<SchoolSubject>> GetSchoolSubjectListByIdsAsync(List<Guid> schoolSubjectIds)
    {
        var schoolSubjectDbos = await context.SchoolSubjects
            .Where(x => schoolSubjectIds.Contains(x.Id))
            .ToListAsync();
        return schoolSubjectDbos.ToSchoolSubjects();
    }

    public async Task AddAsync(SchoolSubject schoolSubject, int scheduleGroupId)
    {
        var schoolSubjectDbo = schoolSubject.ToSchoolSubjectDbo();
        var scheduleGroup = await context.ScheduleGroups
            .Where(x => x.Id == scheduleGroupId)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new InvalidOperationException();
        schoolSubjectDbo.ScheduleGroupId = scheduleGroupId;
        await context.SchoolSubjects.AddAsync(schoolSubjectDbo);
    }

    public async Task UpdateAsync(SchoolSubject schoolSubject)
    {
        var schoolSubjectDbo = await context.SchoolSubjects.FirstOrDefaultAsync(x => x.Id == schoolSubject.Id);
        if (schoolSubjectDbo is null)
            throw new InvalidOperationException();
        DboMapper.Mapper.Map(schoolSubject, schoolSubjectDbo);
    }

    public async Task Delete(SchoolSubject schoolSubject)
    {
        await context.SchoolSubjects.Where(x => x.Id == schoolSubject.Id).ExecuteDeleteAsync();
    }
}