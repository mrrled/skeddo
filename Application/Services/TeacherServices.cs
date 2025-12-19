using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain;
using Domain.Models;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TeacherServices(
    ITeacherRepository teacherRepository,
    ISchoolSubjectRepository schoolSubjectRepository,
    IStudyGroupRepository studyGroupRepository,
    IUnitOfWork  unitOfWork,
    ILogger<TeacherServices> logger
    ) : BaseService(unitOfWork, logger), ITeacherServices
{
    public async Task<List<TeacherDto>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await teacherRepository.GetTeacherListAsync(1);
        return teacherList.ToTeachersDto();
    }

    public async Task<Result<TeacherDto>> GetTeacherById(Guid id)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(id);
        if (teacher is null)
            return Result<TeacherDto>.Failure("Учитель не найден.");
        return Result<TeacherDto>.Success(teacher.ToTeacherDto());
    }

    public async Task<Result<TeacherDto>> AddTeacher(CreateTeacherDto teacherDto)
    {
        var schoolSubjects =
            await schoolSubjectRepository.GetSchoolSubjectListByIdsAsync(teacherDto.SchoolSubjects.Select(x => x.Id)
                .Distinct().ToList());
        var studyGroups =
            await studyGroupRepository.GetStudyGroupListByIdsAsync(teacherDto.StudyGroups.Select(x => x.Id).Distinct()
                .ToList());
        var teacherId = Guid.NewGuid();
        var teacherCreateResult = Teacher.CreateTeacher(teacherId, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, schoolSubjects, studyGroups);
        if (teacherCreateResult.IsFailure)
            return Result<TeacherDto>.Failure(teacherCreateResult.Error);
        var addResult = await ExecuteRepositoryTask(() => teacherRepository.AddAsync(teacherCreateResult.Value, 1),
            "Ошибка при добавлении учителя. Попробуйте позже.");
        if (addResult.IsFailure)
            return Result<TeacherDto>.Failure(addResult.Error);
        return await TrySaveChangesAsync(teacherCreateResult.Value.ToTeacherDto(),
            "Не удалось сохранить учителя. Попробуйте позже.");
    }

    public async Task<Result> EditTeacher(TeacherDto teacherDto)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        if (teacher is null)
            return Result.Failure("Учитель не найден.");
        var subjectIds = teacherDto.SchoolSubjects.Select(x => x.Id).Distinct().ToList();
        var groupIds = teacherDto.StudyGroups.Select(x => x.Id).Distinct().ToList();
        var schoolSubjects = await schoolSubjectRepository.GetSchoolSubjectListByIdsAsync(subjectIds);
        var studyGroups = await studyGroupRepository.GetStudyGroupListByIdsAsync(groupIds);
        if (schoolSubjects.Count != subjectIds.Count)
            return Result.Failure("Некоторые выбранные предметы не найдены.");
        if (studyGroups.Count != groupIds.Count)
            return Result.Failure("Некоторые выбранные группы не найдены.");
        var renameResult = teacher.SetName(teacherDto.Name);
        if (renameResult.IsFailure)
            return Result.Failure(renameResult.Error);
        var surnameEditResult = teacher.SetSurname(teacherDto.Surname);
        if (surnameEditResult.IsFailure)
            return Result.Failure(surnameEditResult.Error);
        var patronymicEditResult = teacher.SetPatronymic(teacherDto.Patronymic);
        if (patronymicEditResult.IsFailure)
            return Result.Failure(patronymicEditResult.Error); 
        teacher.SetDescription(teacherDto.Description);
        teacher.SetSchoolSubjects(schoolSubjects);
        teacher.SetStudyGroups(studyGroups);
        var updateResult = await ExecuteRepositoryTask(() => teacherRepository.UpdateAsync(teacher),
            "Ошибка при изменении учителя. Попробуйте позже.");
        if (updateResult.IsFailure)
            return updateResult;
        return await TrySaveChangesAsync("Не удалось изменить учителя. Попробуйте позже.");
    }

    public async Task<Result> DeleteTeacher(TeacherDto teacherDto)
    {
        var teacher = await teacherRepository.GetTeacherByIdAsync(teacherDto.Id);
        if (teacher is null)
            return Result.Failure("Учитель не найден.");
        await teacherRepository.Delete(teacher);
        return await TrySaveChangesAsync("Не удалось удалить учителя. Попробуйте позже.");
    }
}