using Domain.Models;

namespace Domain.IRepositories;

public interface IStudyGroupRepository
{
    Task<List<StudyGroup>> GetStudyGroupListAsync();
    Task<List<StudyGroup>> GetStudyGroupListByScheduleIdAsync(Guid scheduleId);
    Task<StudyGroup?> GetStudyGroupByIdAsync(Guid studyGroupId);
    Task<List<StudyGroup>> GetStudyGroupListByIdsAsync(List<Guid> studyGroupIds);
    Task AddAsync(StudyGroup studyGroup, Guid scheduleId);
    Task UpdateAsync(StudyGroup studyGroup);
    Task Delete(StudyGroup studyGroup);
}