using Infrastructure.DboModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ScheduleDbContext : DbContext
{
    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<DboTeacher> Teachers { get; set; }
    public DbSet<DboTeacherSubject> TeacherSubjects { get; set; }
    public DbSet<DboTeacherStudyGroup> TeacherStudyGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DboTeacherSubject>().HasNoKey();
        modelBuilder.Entity<DboTeacherStudyGroup>().HasNoKey();
        modelBuilder.Entity<DboSchoolSubject>().HasNoKey();
        modelBuilder.Entity<DboStudyGroup>().HasNoKey();
        modelBuilder.Entity<DboClassroom>().HasNoKey();
    }
}