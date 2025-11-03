namespace Domain.Models;

public class Schedule(
    int id,
    HashSet<Lesson> lessons
) : Entity<int>(id)
{
    private HashSet<Lesson> lessons = lessons;
    //таблица будет валидировать уроки
}