using Domain.Models;
using Domain.Repositories;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TeacherRepository(ScheduleDbContext context) : ITeacherRepository
{
    public async Task<List<Teacher>> GetTeacherListAsync()
    {
        var teachers = await context.Teachers
            .Where(x => x.ScheduleGroupId == 1)
            .Include(teacherDbo => teacherDbo.SchoolSubjects)
            .Include(teacherDbo => teacherDbo.StudyGroups)
            .ToListAsync();

        return teachers.ToTeachers();
    }

    public async Task<Teacher> GetTeacherByIdAsync(int id)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Teachers)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var teacher = scheduleGroup.Teachers.FirstOrDefault(x => x.Id == id);
        if (teacher is null)
            throw new NullReferenceException();
        return teacher.ToTeacher();
    }

    public async Task AddAsync(Teacher teacher)
    {
        var teacherDbo = teacher.ToTeacherDbo();
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.Teachers.Add(teacherDbo);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        var teacherDbo = await context.Teachers.FirstOrDefaultAsync(x => x.Id == teacher.Id);
        if (teacherDbo is null)
            throw new ArgumentException();
        DboMapper.Mapper.Map(teacher, teacherDbo);
    }

    public async Task Delete(Teacher teacher)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Teachers)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var teacherDbo = scheduleGroup.Teachers.FirstOrDefault(x => x.Id == teacher.Id);
        if (teacherDbo is null)
            throw new NullReferenceException();
        scheduleGroup.Teachers.Remove(teacherDbo);
    }
}