using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SchoolSubjectServices(
    ISchoolSubjectRepository schoolSubjectRepository,
    IUnitOfWork unitOfWork,
    ILogger logger) : BaseService(unitOfWork, logger), ISchoolSubjectServices
{
    public async Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync()
    {
        var schoolSubjectList = await schoolSubjectRepository.GetSchoolSubjectListAsync(1);
        return schoolSubjectList.ToSchoolSubjectsDto();
    }

    public async Task<Result<SchoolSubjectDto>> AddSchoolSubject(CreateSchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubjectId = Guid.NewGuid();
        var schoolSubjectCreateResult = SchoolSubject.CreateSchoolSubject(schoolSubjectId, schoolSubjectDto.Name);
        if (schoolSubjectCreateResult.IsFailure)
            return Result<SchoolSubjectDto>.Failure(schoolSubjectCreateResult.Error);
        var addResult = await ExecuteRepositoryTask(
            () => schoolSubjectRepository.AddAsync(schoolSubjectCreateResult.Value, 1),
            "Ошибка при добавлении предмета. Попробуйте позже.");
        if (addResult.IsFailure)
            return Result<SchoolSubjectDto>.Failure(addResult.Error);
        return await TrySaveChangesAsync(schoolSubjectCreateResult.Value.ToSchoolSubjectDto(),
            "Не удалось сохранить предмет. Попробуйте позже.");
    }

    public async Task<Result> EditSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id);
        if (schoolSubject is null)
            return Result.Failure("Предмет не найден.");
        if (schoolSubject.Name != schoolSubjectDto.Name)
        {
            var renameResult = schoolSubject.SetName(schoolSubjectDto.Name);
            if (renameResult.IsFailure)
                return renameResult;
        }

        var updateResult = await ExecuteRepositoryTask(() => schoolSubjectRepository.UpdateAsync(schoolSubject),
            "Ошибка при изменении предмета. Попробуйте позже.");
        if (updateResult.IsFailure)
            return updateResult;
        return await TrySaveChangesAsync("Не удалось изменить предмет. Попробуйте позже.");
    }

    public async Task<Result> DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id);
        if (schoolSubject is null)
            return Result.Failure("Предмет не найден.");
        await schoolSubjectRepository.Delete(schoolSubject);
        return await TrySaveChangesAsync("Не удалось удалить предмет. Попробуйте позже.");
    }
}