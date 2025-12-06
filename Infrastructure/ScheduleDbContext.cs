using Infrastructure.DboModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ScheduleDbContext : DbContext
{
    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<TeacherDbo> Teachers { get; set; }
    public DbSet<LessonDbo> Lessons { get; set; }
    public DbSet<LessonDraftDbo>  LessonDrafts { get; set; }
    public DbSet<ScheduleDbo> Schedules { get; set; }
    public DbSet<ClassroomDbo> Classrooms { get; set; }
    public DbSet<StudyGroupDbo> StudyGroups { get; set; }
    public DbSet<SchoolSubjectDbo> SchoolSubjects { get; set; }
    public DbSet<ScheduleGroupDbo> ScheduleGroups { get; set; }
    public DbSet<LessonNumberDbo> LessonNumbers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LessonDbo>().Navigation(s => s.Classroom).AutoInclude();
        modelBuilder.Entity<LessonDbo>().Navigation(s => s.SchoolSubject).AutoInclude();
        modelBuilder.Entity<LessonDbo>().Navigation(s => s.StudyGroup).AutoInclude();
        modelBuilder.Entity<LessonDbo>().Navigation(s => s.Teacher).AutoInclude();
        modelBuilder.Entity<LessonDbo>().Navigation(s => s.LessonNumber).AutoInclude();
        base.OnModelCreating(modelBuilder);
    }
}