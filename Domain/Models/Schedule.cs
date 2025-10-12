namespace Domain.Models;

public class Schedule
{
    public int Id { get; set; }
    private HashSet<Lesson> _lessons;
    //таблица будет валидировать уроки
}