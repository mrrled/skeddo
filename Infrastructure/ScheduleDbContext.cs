using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ScheduleDbContext : DbContext
{
    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<TeacherSubjectModel> TeacherSubjects { get; set; }
    public DbSet<TeacherStudyGroupModel> TeacherStudyGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeacherSubjectModel>().HasNoKey();
        modelBuilder.Entity<TeacherStudyGroupModel>().HasNoKey();
        modelBuilder.Entity<SchoolSubjectEntity>().HasNoKey();
        modelBuilder.Entity<StudyGroupEntity>().HasNoKey();
        modelBuilder.Entity<ClassroomEntity>().HasNoKey();
    }
}