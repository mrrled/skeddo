namespace Domain.Models;

public class Schedule
{
    public Guid Id { get; set; }
    private List<Lesson> _lessons;
    //таблица будет валидировать уроки
}