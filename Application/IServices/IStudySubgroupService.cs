using Domain.Models;

namespace Application.IServices;

public interface IStudySubgroupService
{
    public Task AddStudySubgroup(StudySubgroup studySubgroup, int scheduleId);
    public Task EditStudySubgroup(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, int scheduleId);
    public Task DeleteStudySubgroup(StudySubgroup studySubgroup, int scheduleId);
}