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
        base.OnModelCreating(modelBuilder);
    }
}