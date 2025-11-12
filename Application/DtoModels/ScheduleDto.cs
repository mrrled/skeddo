using Domain.Models;

namespace Application.DtoModels;

public class ScheduleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Lesson> Lessons { get; set; } = new List<Lesson>();
}