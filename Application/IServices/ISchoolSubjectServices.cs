using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface ISchoolSubjectServices
{
    public Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync();
    public Task<Result<SchoolSubjectDto>> AddSchoolSubject(CreateSchoolSubjectDto schoolSubjectDto);
    public Task<Result> EditSchoolSubject(SchoolSubjectDto subjectSchoolSubjectDto);
    public Task<Result> DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto);
}