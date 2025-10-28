using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    public static abstract List<Teacher> GetTeachers();
}