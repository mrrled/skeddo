using Domain.Models;

namespace Domain.IRepositories;

public interface IStudyGroupRepository
{
    Task<List<StudyGroup>> GetStudyGroupListAsync(int scheduleGroupId);
    Task<StudyGroup> GetStudyGroupByIdAsync(int studyGroupId);
    Task<List<StudyGroup>> GetStudyGroupListByIdsAsync(List<int> studyGroupIds);
    Task AddAsync(StudyGroup studyGroup, int scheduleGroupId);
    Task UpdateAsync(StudyGroup studyGroup);
    Task Delete(StudyGroup studyGroup);
}