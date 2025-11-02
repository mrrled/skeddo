using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly ScheduleDbContext _context;

    public ScheduleRepository(ScheduleDbContext context)
    {
        _context = context;
    }

    public List<Classroom> GetClassrooms() => new();
    public List<Lesson> GetLessons() => new();
    public List<Schedule> GetSchedules() => new();
    public List<SchoolSubject> GetSchoolSubjects() => new();
    public List<StudyGroup> GetStudyGroups() => new();

    public List<Teacher> GetTeachers()
    {
        var teachers = _context.Teachers.Where(x => x.GroupId == 1).ToList();
        var teacherSubjects = _context
            .TeacherSubjects
            .Where(x => x.GroupId == 1)
            .ToList()
            .GroupBy(x => x.TeacherId)
            .ToDictionary(x => x.Key,
                x => x.Select(t => t.SchoolSubject)
                    .ToList());
        ;

        var result = teachers.Select(x =>
        {
            var nameParts = x.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var surname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var name = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            var patronymic = nameParts.Length > 2 ? nameParts[2] : string.Empty;

            var subjects = teacherSubjects.ContainsKey(x.Id)
                ? teacherSubjects[x.Id].Select(s => new SchoolSubject(s)).ToList()
                : new List<SchoolSubject>();

            return new Teacher(
                x.Id,
                name,
                surname,
                patronymic,
                subjects,
                new List<StudyGroup>()
            );
        }).ToList();

        return result;
    }

    public List<TimeSlot> GetTimeSlots() => new();
}