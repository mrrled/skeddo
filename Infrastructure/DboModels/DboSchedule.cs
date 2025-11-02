using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Schedules")]
public class DboSchedule
{
    public int Id { get; set; }
    public int GroupId { get; set; }
}