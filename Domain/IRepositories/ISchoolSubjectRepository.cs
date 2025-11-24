using Domain.Models;

namespace Domain.IRepositories;

public interface ISchoolSubjectRepository
{
    Task<List<SchoolSubject>> GetSchoolSubjectListAsync();
    Task AddAsync(SchoolSubject schoolSubject);
    Task UpdateAsync(SchoolSubject oldSchoolSubject, SchoolSubject newSchoolSubject);
    Task Delete(SchoolSubject schoolSubject);
}