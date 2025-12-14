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
        var schoolSubjectList = await schoolSubjectRepository.GetSchoolSubjectListAsync(1);
        return schoolSubjectList.ToSchoolSubjectsDto();
    }

    public async Task<SchoolSubjectDto> AddSchoolSubject(CreateSchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubjectId = Guid.NewGuid();
        var schoolSubject = SchoolSubject.CreateSchoolSubject(schoolSubjectId, schoolSubjectDto.Name);
        await schoolSubjectRepository.AddAsync(schoolSubject, 1);
        await unitOfWork.SaveChangesAsync();
        return schoolSubject.ToSchoolSubjectDto();
    }

    public async Task EditSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id);
        if (schoolSubject is null)
            throw new  ArgumentException($"School subject with id {schoolSubjectDto.Id} not found");
        if (schoolSubject.Name != schoolSubjectDto.Name)
            schoolSubject.SetName(schoolSubjectDto.Name);
        await schoolSubjectRepository.UpdateAsync(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = await schoolSubjectRepository.GetSchoolSubjectByIdAsync(schoolSubjectDto.Id);
        if (schoolSubject is null)
            throw new  ArgumentException($"School subject with id {schoolSubjectDto.Id} not found");
        await schoolSubjectRepository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}