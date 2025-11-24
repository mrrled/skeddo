using Domain.Models;

namespace Domain.IRepositories;

public interface IClassroomRepository
{
    Task<List<Classroom>> GetClassroomListAsync();
    Task AddAsync(Classroom classroom);
    Task UpdateAsync(Classroom oldClassroom, Classroom newClassroom);
    Task Delete(Classroom classroom);
}