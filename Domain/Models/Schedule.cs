namespace Domain.Models;

public class Schedule(
    int id,
    HashSet<Lesson> lessons
)
{
    public int Id { get; set; } = id;

    private HashSet<Lesson> _lessons = lessons;
    //таблица будет валидировать уроки
}