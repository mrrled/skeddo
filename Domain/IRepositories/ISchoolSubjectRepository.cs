using Domain.Models;

namespace Domain.IRepositories;

public interface ISchoolSubjectRepository
{
    Task<List<SchoolSubject>> GetSchoolSubjectListAsync(int scheduleGroupId);
    Task<SchoolSubject> GetSchoolSubjectByIdAsync(Guid schoolSubjectId);
    Task<List<SchoolSubject>> GetSchoolSubjectListByIdsAsync(List<Guid> schoolSubjectIds);
    Task AddAsync(SchoolSubject schoolSubject, int scheduleGroupId);
    Task UpdateAsync(SchoolSubject schoolSubject);
    Task Delete(SchoolSubject schoolSubject);
}