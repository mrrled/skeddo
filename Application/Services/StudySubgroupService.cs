using Application.DtoModels;
using Application.IServices;
using Domain;
using Domain.IRepositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class StudySubgroupService(
    IStudyGroupRepository studyGroupRepository,
    IStudySubgroupRepository studySubgroupRepository,
    IUnitOfWork unitOfWork,
    ILogger logger) : BaseService(unitOfWork, logger), IStudySubgroupService
{
    public async Task<Result> AddStudySubgroup(StudySubgroupDto studySubgroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id);
        if (studyGroup is null)
            return Result.Failure("Учебная группа не найдена.");
        var studySubgroupCreateResult = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name);
        if (studySubgroupCreateResult.IsFailure)
            return Result.Failure(studySubgroupCreateResult.Error);
        var addResult = await ExecuteRepositoryTask(
            () => studySubgroupRepository.AddAsync(studySubgroupCreateResult.Value, studyGroup.Id),
            "Ошибка при добавлении учебной подгруппы. Попробуйте позже.");
        if (addResult.IsFailure)
            return addResult;
        return await TrySaveChangesAsync("Не удалось сохранить учебную подгруппу. Попробуйте позже.");
    }

    public async Task<Result> EditStudySubgroup(StudySubgroupDto oldStudySubgroupDto,
        StudySubgroupDto newStudySubgroupDto)
    {
        if (oldStudySubgroupDto.StudyGroup.Id != newStudySubgroupDto.StudyGroup.Id)
            return Result.Failure("Смена учебной группы запрещена.");
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(oldStudySubgroupDto.StudyGroup.Id);
        if (studyGroup is null)
            return Result.Failure("Учебная группа не найдена.");
        var oldStudySubgroupCreateResult = StudySubgroup.CreateStudySubgroup(studyGroup, oldStudySubgroupDto.Name);
        if (oldStudySubgroupCreateResult.IsFailure)
            return Result.Failure(oldStudySubgroupCreateResult.Error);
        var newStudySubgroupCreateResult = StudySubgroup.CreateStudySubgroup(studyGroup, newStudySubgroupDto.Name);
        if (newStudySubgroupCreateResult.IsFailure)
            return Result.Failure(newStudySubgroupCreateResult.Error);
        var updateResult = await ExecuteRepositoryTask(
            () => studySubgroupRepository.UpdateAsync(oldStudySubgroupCreateResult.Value,
                newStudySubgroupCreateResult.Value, studyGroup.Id),
            "Ошибка при изменении учебной подгруппы. Попробуйте позже.");
        if (updateResult.IsFailure)
            return updateResult;
        return await TrySaveChangesAsync("Не удалось изменить учебную подгруппу. Попробуйте позже.");
    }

    public async Task<Result> DeleteStudySubgroup(StudySubgroupDto studySubgroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id);
        if (studyGroup is null)
            return Result.Failure("Учебная группа не найдена.");
        var studySubgroupCreateResult = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name);
        if (studySubgroupCreateResult.IsFailure)
            return Result.Failure(studySubgroupCreateResult.Error);
        await studySubgroupRepository.Delete(studySubgroupCreateResult.Value, studyGroup.Id);
        return await TrySaveChangesAsync("Не удалось удалить учебную подгруппу. Попробуйте позже.");
    }
}