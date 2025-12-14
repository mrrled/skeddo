using Domain.Models;

namespace Domain.IRepositories;

public interface IStudySubgroupRepository
{
    Task AddAsync(StudySubgroup studySubgroup, Guid studyGroupId);
    Task UpdateAsync(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, Guid studyGroupId);
    Task Delete(StudySubgroup studySubgroup, Guid studyGroupId);
}