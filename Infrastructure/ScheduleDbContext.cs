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
    public DbSet<ScheduleDbo> Schedules { get; set; }
    public DbSet<ScheduleGroupDbo> ScheduleGroups { get; set; }

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