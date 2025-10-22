using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    List<Teacher> GetTeachers();
}