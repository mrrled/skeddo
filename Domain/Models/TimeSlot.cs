namespace Domain.Models;

public struct TimeSlot
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}