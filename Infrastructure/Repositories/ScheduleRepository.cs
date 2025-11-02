using AutoMapper;
using Domain;
using Domain.Models;
using Infrastructure.DboExtensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    
    private readonly ScheduleDbContext _context;
    private readonly IMapper mapper;

    public ScheduleRepository(ScheduleDbContext context, IMapper mapper)
    {
        _context = context;
        this.mapper = mapper;
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
            .AsEnumerable()
            .GroupBy(x => x.TeacherId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(t => new SchoolSubject(t.SchoolSubject)).ToList()
            );
    
        var teacherStudyGroups = _context
            .TeacherStudyGroups
            .Where(x => x.GroupId == 1)
            .AsEnumerable()
            .GroupBy(x => x.TeacherId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(t => new StudyGroup(t.StudyGroup)).ToList()
            );
        
        return teachers.ToTeacher(mapper, opts => 
        {
            opts.Items["SpecializationsByTeacher"] = teacherSubjects;
            opts.Items["StudyGroupsByTeacher"] = teacherStudyGroups;
        });
    }

    public List<TimeSlot> GetTimeSlots() => new();
}