using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ISchoolSubjectServices
{
    public Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync();
    public Task<SchoolSubjectDto> AddSchoolSubject(CreateSchoolSubjectDto schoolSubjectDto);
    public Task EditSchoolSubject(SchoolSubjectDto subjectSchoolSubjectDto);
    public Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto);
}