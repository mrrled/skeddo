using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class ClassroomServices(IClassroomRepository classroomRepository, IUnitOfWork unitOfWork) : IClassroomServices
{
    public async Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await classroomRepository.GetClassroomListAsync(1);
        return classroomList.ToClassroomsDto();
    }

    public async Task AddClassroom(ClassroomDto classroomDto)
    {
        var classroom = Classroom.CreateClassroom(classroomDto.Id, classroomDto.Name, classroomDto.Description);
        await classroomRepository.AddAsync(classroom, 1);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditClassroom(ClassroomDto classroomDto)
    {
        var classroom = await classroomRepository.GetClassroomByIdAsync(classroomDto.Id);
        if (classroom is null)
            throw new ArgumentException($"Classroom with id {classroomDto.Id} not found");
        if (classroomDto.Name != classroom.Name)
            classroom.SetName(classroomDto.Name);
        if (classroomDto.Description != classroom.Description)
            classroom.SetDescription(classroomDto.Description);
        await classroomRepository.UpdateAsync(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteClassroom(ClassroomDto classroomDto)
    {
        var classroom = await classroomRepository.GetClassroomByIdAsync(classroomDto.Id);
        if (classroom is null)
            throw new ArgumentException($"Classroom with id {classroomDto.Id} not found");
        await classroomRepository.Delete(classroom);
        await unitOfWork.SaveChangesAsync();
    }
}