using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    public List<Teacher> GetTeachers();
}