using Domain.Models;

namespace Domain.IRepositories;

public interface ISchoolSubjectRepository
{
    Task<List<SchoolSubject>> GetSchoolSubjectListAsync(int scheduleGroupId);
    Task<SchoolSubject> GetSchoolSubjectByIdAsync(int schoolSubjectId);
    Task<List<SchoolSubject>> GetSchoolSubjectListByIdsAsync(List<int> schoolSubjectIds);
    Task AddAsync(SchoolSubject schoolSubject, int scheduleGroupId);
    Task UpdateAsync(SchoolSubject schoolSubject);
    Task Delete(SchoolSubject schoolSubject);
}