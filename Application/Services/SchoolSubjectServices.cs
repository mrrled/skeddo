using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class SchoolSubjectServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : ISchoolSubjectServices
{
    public async Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync()
    {
        var schoolSubjectList = await repository.GetSchoolSubjectListAsync();
        return schoolSubjectList.ToSchoolSubjectDto();
    }

    public async Task AddSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.AddAsync(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchoolSubject(SchoolSubjectDto oldSubjectSchoolSubjectDto,
        SchoolSubjectDto newSubjectSchoolSubjectDto)
    {
        var oldSchoolSubject = Schedule.CreateSchoolSubject(oldSubjectSchoolSubjectDto.Name);
        var newSchoolSubject = Schedule.CreateSchoolSubject(newSubjectSchoolSubjectDto.Name);
        await repository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}