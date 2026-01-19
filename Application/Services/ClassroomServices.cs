using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ClassroomServices(
    IClassroomRepository classroomRepository,
    IUnitOfWork unitOfWork,
    ILogger<ClassroomServices> logger)
    : BaseService(unitOfWork, logger), IClassroomServices
{
    public async Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await classroomRepository.GetClassroomListAsync(1);
        return classroomList.ToClassroomsDto();
    }

    public async Task<Result<ClassroomDto>> AddClassroom(CreateClassroomDto classroomDto)
    {
        var classroomId = Guid.NewGuid();
        var classroomResult = Classroom.CreateClassroom(classroomId, classroomDto.Name, classroomDto.Description);
        if (classroomResult.IsFailure)
            return Result<ClassroomDto>.Failure(classroomResult.Error);
        var addResult = await ExecuteRepositoryTask(() => classroomRepository.AddAsync(classroomResult.Value, 1),
            "Ошибка при добавлении аудитории. Попробуйте позже.");
        if (addResult.IsFailure)
            return Result<ClassroomDto>.Failure(addResult.Error);
        return await TrySaveChangesAsync(classroomResult.Value.ToClassroomDto(),
            "Не удалось сохранить аудиторию. Попробуйте позже.");
    }

    public async Task<Result> EditClassroom(ClassroomDto classroomDto)
    {
        var classroom = await classroomRepository.GetClassroomByIdAsync(classroomDto.Id);
        if (classroom is null)
            return Result.Failure("Аудитория не найдена.");
        if (classroomDto.Name != classroom.Name)
        {
            var renameResult = classroom.SetName(classroomDto.Name);
            if (renameResult.IsFailure)
                return renameResult;
        }

        if (classroomDto.Description != classroom.Description)
            classroom.SetDescription(classroomDto.Description);
        var updateResult = await ExecuteRepositoryTask(() => classroomRepository.UpdateAsync(classroom),
            "Ошибка изменении аудитории. Попробуйте позже");
        if (updateResult.IsFailure)
            return updateResult;
        return await TrySaveChangesAsync("Не удалось изменить аудиторию. Попробуйте позже.");
    }

    public async Task<Result> DeleteClassroom(ClassroomDto classroomDto)
    {
        var classroom = await classroomRepository.GetClassroomByIdAsync(classroomDto.Id);
        if (classroom is null)
            return Result.Failure("Аудитория не найдена.");
        await classroomRepository.Delete(classroom);
        return await TrySaveChangesAsync("Не удалось удалить аудиторию. Попробуйте позже.");
    }
}