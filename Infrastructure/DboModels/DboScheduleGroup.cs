using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("ScheduleGroups")]
public class DboScheduleGroup
{
    public int Id { get; set; }
}