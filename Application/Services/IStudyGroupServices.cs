using Application.DtoModels;
using Domain.Models;

namespace Application.Services;

public interface IStudyGroupServices
{
    public Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync();
    public Task AddStudyGroup(StudyGroupDto studyGroupDto);
    public Task EditStudyGroup(StudyGroupDto oldStudyGroupDto, StudyGroupDto newStudyGroupDto);
    public Task DeleteStudyGroup(StudyGroupDto studyGroupDto);
}