using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities;

[PrimaryKey(nameof(Id))]
[Table("ScheduleGroups")]
public class ScheduleGroupEntity
{
    public int Id { get; set; }
}