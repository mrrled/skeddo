using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities;

[PrimaryKey(nameof(Id))]
[Table("Schedules")]
public class ScheduleEntity
{
    public int Id { get; set; }
    public int GroupId { get; set; }
}