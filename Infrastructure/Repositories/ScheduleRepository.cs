using Domain;
using Domain.Models;

namespace Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly ScheduleDbContext _context;

    public ScheduleRepository(ScheduleDbContext context)
    {
        _context = context;
    }

    public List<Teacher> GetTeachers()
    {
        var teachers = _context.Teachers.Where(x => x.GroupId == 1).ToList();
        var teacherIds = teachers.Select(x => x.Id).ToList();
        var teacherSubjects = _context
            .TeacherSubjects
            .Where(x => x.GroupId == 1)
            .GroupBy(x => x.TeacherId)
            .ToDictionary(x => x.Key, x => x.Select(t => t.SchoolSubject).ToList());
        var teacherStudyGroups = _context
            .TeacherStudyGroups
            .Where(x => x.GroupId == 1)
            .GroupBy(x => x.TeacherId)
            .ToDictionary(x => x.Key, x => x.Select(t => t.StudyGroup).ToList());
        var result = teachers.Select(x => new Teacher(
            x.Id,
            x.FullName.Split(" ")[0],
            x.FullName.Split(" ")[1],
            x.FullName.Split(" ")[2],
            new List<SchoolSubject>(),
            new List<StudyGroup>())).ToList();
        return result;
    }
}