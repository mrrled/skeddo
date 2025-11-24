using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class SchoolSubjectServices(ISchoolSubjectRepository schoolSubjectRepository, IUnitOfWork unitOfWork) : ISchoolSubjectServices
{
    public async Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync()
    {
        var schoolSubjectList = await schoolSubjectRepository.GetSchoolSubjectListAsync();
        return schoolSubjectList.ToSchoolSubjectsDto();
    }

    public async Task AddSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await schoolSubjectRepository.AddAsync(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchoolSubject(SchoolSubjectDto oldSubjectSchoolSubjectDto,
        SchoolSubjectDto newSubjectSchoolSubjectDto)
    {
        var oldSchoolSubject = Schedule.CreateSchoolSubject(oldSubjectSchoolSubjectDto.Name);
        var newSchoolSubject = Schedule.CreateSchoolSubject(newSubjectSchoolSubjectDto.Name);
        await schoolSubjectRepository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await schoolSubjectRepository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}