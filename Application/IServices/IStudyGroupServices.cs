using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface IStudyGroupServices
{
    public Task<List<StudyGroupDto>> GetStudyGroupByScheduleId(Guid scheduleId);
    public Task<Result<StudyGroupDto>> AddStudyGroup(CreateStudyGroupDto studyGroupDto);
    public Task<Result> EditStudyGroup(StudyGroupDto studyGroupDto);
    public Task<Result> DeleteStudyGroup(StudyGroupDto studyGroupDto);
}