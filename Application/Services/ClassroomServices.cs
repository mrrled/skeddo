using Application.DtoModels;
using Application.DtoExtensions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class ClassroomServices(IClassroomRepository classroomRepository, IUnitOfWork unitOfWork) : IClassroomServices
{
    public async Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await classroomRepository.GetClassroomListAsync();
        return classroomList.ToClassroomsDto();
    }

    public async Task AddClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await classroomRepository.AddAsync(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto)
    {
        var oldClassroom = Schedule.CreateClassroom(oldClassroomDto.Name, oldClassroomDto.Description);
        var newClassroom = Schedule.CreateClassroom(newClassroomDto.Name, newClassroomDto.Description);
        await classroomRepository.UpdateAsync(oldClassroom, newClassroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await classroomRepository.Delete(classroom);
        await unitOfWork.SaveChangesAsync();
    }
}