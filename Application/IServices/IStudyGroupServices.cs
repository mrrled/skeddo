using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface IStudyGroupServices
{
    public Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync();
    public Task AddStudyGroup(StudyGroupDto studyGroupDto);
    public Task EditStudyGroup(StudyGroupDto studyGroupDto);
    public Task DeleteStudyGroup(StudyGroupDto studyGroupDto);
}