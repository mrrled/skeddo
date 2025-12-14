using Domain.Models;
using Domain.IRepositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TeacherRepository(ScheduleDbContext context) : ITeacherRepository
{
    public async Task<List<Teacher>> GetTeacherListAsync(int scheduleGroupId)
    {
        var teachers = await context.Teachers
            .Where(x => x.ScheduleGroupId == scheduleGroupId)
            .Include(teacherDbo => teacherDbo.SchoolSubjects)
            .Include(teacherDbo => teacherDbo.StudyGroups)
            .ToListAsync();

        return teachers.ToTeachers();
    }

    public async Task<Teacher> GetTeacherByIdAsync(Guid id)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == id);
        if (teacher is null)
            throw new InvalidOperationException();
        return teacher.ToTeacher();
    }

    public async Task AddAsync(Teacher teacher, int scheduleGroupId)
    {
        var teacherDbo = teacher.ToTeacherDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new InvalidOperationException();
        teacherDbo.ScheduleGroupId = scheduleGroupId;
        context.Teachers.Add(teacherDbo);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        var teacherDbo = await context.Teachers.FirstOrDefaultAsync(x => x.Id == teacher.Id);
        if (teacherDbo is null)
            throw new InvalidOperationException();
        DboMapper.Mapper.Map(teacher, teacherDbo);
    }

    public async Task Delete(Teacher teacher)
    {
        await context.Teachers.Where(x => x.Id == teacher.Id).ExecuteDeleteAsync();
    }
}