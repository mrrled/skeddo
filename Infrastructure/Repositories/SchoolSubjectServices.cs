using Domain.Models;
using Domain.Repositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SchoolSubjectRepository(ScheduleDbContext context) : ISchoolSubjectRepository
{
    public async Task<List<SchoolSubject>> GetSchoolSubjectListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.SchoolSubjects)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.SchoolSubjects.ToSchoolSubjects();
    }

    public async Task AddAsync(SchoolSubject schoolSubject)
    {
        var schoolSubjectDbo = schoolSubject.ToSchoolSubjectDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.SchoolSubjects.Add(schoolSubjectDbo);
    }

    public async Task UpdateAsync(SchoolSubject oldSchoolSubject, SchoolSubject newSchoolSubject)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schoolSubjectDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == oldSchoolSubject.Name);
        if (schoolSubjectDbo is null)
            throw new NullReferenceException();
        DboMapper.Mapper.Map(newSchoolSubject, schoolSubjectDbo);
    }

    public async Task Delete(SchoolSubject schoolSubject)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.SchoolSubjects)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schoolSubjectDbo = scheduleGroup.SchoolSubjects.FirstOrDefault(x => x.Name == schoolSubject.Name);
        if (schoolSubjectDbo is null)
            throw new NullReferenceException();
        scheduleGroup.SchoolSubjects.Remove(schoolSubjectDbo);
    }
}