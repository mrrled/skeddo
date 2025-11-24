using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class ClassroomServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : IClassroomServices
{
    public async Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await repository.GetClassroomListAsync();
        return classroomList.ToClassroomDto();
    }

    public async Task AddClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.AddAsync(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto)
    {
        var oldClassroom = Schedule.CreateClassroom(oldClassroomDto.Name, oldClassroomDto.Description);
        var newClassroom = Schedule.CreateClassroom(newClassroomDto.Name, newClassroomDto.Description);
        await repository.UpdateAsync(oldClassroom, newClassroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.Delete(classroom);
        await unitOfWork.SaveChangesAsync();
    }
}