using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface ISchoolSubjectServices
{
    public Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync();
    public Task AddSchoolSubject(SchoolSubjectDto schoolSubjectDto);
    public Task EditSchoolSubject(SchoolSubjectDto oldSubjectSchoolSubjectDto, SchoolSubjectDto newSubjectSchoolSubjectDto);
    public Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto);
}