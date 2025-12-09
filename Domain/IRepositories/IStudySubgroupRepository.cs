using Domain.Models;

namespace Domain.IRepositories;

public interface IStudySubgroupRepository
{
    Task AddAsync(StudySubgroup studySubgroup, int studyGroupId);
    Task UpdateAsync(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, int studyGroupId);
    Task Delete(StudySubgroup studySubgroup, int studyGroupId);
}