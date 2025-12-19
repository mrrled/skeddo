using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class StudyGroupServices(IStudyGroupRepository studyGroupRepository, IUnitOfWork unitOfWork, ILogger<StudyGroupServices> logger) : BaseService(unitOfWork, logger), IStudyGroupServices
{
    public async Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await studyGroupRepository.GetStudyGroupListAsync(1);
        return studyGroupList.ToStudyGroupsDto();
    }

    public async Task<Result<StudyGroupDto>> AddStudyGroup(CreateStudyGroupDto studyGroupDto)
    {
        var studyGroupId = Guid.NewGuid();
        var studyGroupCreateResult = StudyGroup.CreateStudyGroup(studyGroupId, studyGroupDto.Name);
        if (studyGroupCreateResult.IsFailure)
            return Result<StudyGroupDto>.Failure(studyGroupCreateResult.Error);
        var addResult = await ExecuteRepositoryTask(
            () => studyGroupRepository.AddAsync(studyGroupCreateResult.Value, 1),
            "Ошибка при добавлении учебной группы. Попробуйте позже.");
        if (addResult.IsFailure)
            return Result<StudyGroupDto>.Failure(addResult.Error);
        return await TrySaveChangesAsync(studyGroupCreateResult.Value.ToStudyGroupDto(),
            "Не удалось сохранить учебную группу. Попробуйте позже.");
    }

    public async Task<Result> EditStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studyGroupDto.Id);
        if (studyGroup is null)
            return Result.Failure("Учебная группа не найдена.");
        if (studyGroupDto.Name != studyGroup.Name)
        {
            var renameResult = studyGroup.SetName(studyGroupDto.Name);
            if (renameResult.IsFailure)
                return renameResult;
        }

        var updateResult = await ExecuteRepositoryTask(() => studyGroupRepository.UpdateAsync(studyGroup),
            "Ошибка при изменении учебной группы. Попробуйте позже.");
        if (updateResult.IsFailure)
            return updateResult;
        return await TrySaveChangesAsync("Не удалось изменить учебную группу. Попробуйте позже.");
    }

    public async Task<Result> DeleteStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studyGroupDto.Id);
        if (studyGroup is null)
            return Result.Failure("Учебная группа не найдена.");
        await studyGroupRepository.Delete(studyGroup);
        return await TrySaveChangesAsync("Не удалось удалить учебную группу. Попробуйте позже.");
    }
}