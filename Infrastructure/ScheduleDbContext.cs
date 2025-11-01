using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ScheduleDbContext : DbContext
{
    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<TeacherDbo> Teachers { get; set; }
    public DbSet<TeacherSubjectDbo> TeacherSubjects { get; set; }
    public DbSet<TeacherStudyGroupDbo> TeacherStudyGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeacherSubjectDbo>().HasNoKey();
        modelBuilder.Entity<TeacherStudyGroupDbo>().HasNoKey();
        modelBuilder.Entity<SchoolSubjectDbo>().HasNoKey();
        modelBuilder.Entity<StudyGroupDbo>().HasNoKey();
        modelBuilder.Entity<ClassroomDbo>().HasNoKey();
    }
}