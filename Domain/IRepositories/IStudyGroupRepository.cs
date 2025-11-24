using Domain.Models;

namespace Domain.IRepositories;

public interface IStudyGroupRepository
{
    Task<List<StudyGroup>> GetStudyGroupListAsync();
    Task AddAsync(StudyGroup studyGroup);
    Task UpdateAsync(StudyGroup oldStudyGroup, StudyGroup newStudyGroup);
    Task Delete(StudyGroup studyGroup);
}