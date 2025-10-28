using Domain;
using Domain.Models;

namespace Infrastructure.Repositories;

public class TeacherRepository : IScheduleRepository
{
    public List<Teacher> GetTeachers()
    {
        return Teachers;
    }

    private static readonly List<Teacher> Teachers =
    [
        new Teacher(42,
            "Иван", "Иванов", "Иванович",
            [new SchoolSubject("математика")],
            []),


        new Teacher(52,
            "Имя", "Фамилия", "Отчество",
            [new SchoolSubject("оригами")],
            []),


        new Teacher(86,
            "Абоба", "Чиназес", "Чиловович",
            [new SchoolSubject("пинает всякое разное")],
            [])
    ];
}