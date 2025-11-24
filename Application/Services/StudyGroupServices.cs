using Application.DtoModels;
using Application.Extensions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class StudyGroupServices(IStudyGroupRepository studyGroupRepository, IUnitOfWork unitOfWork) : IStudyGroupServices
{
    public async Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await studyGroupRepository.GetStudyGroupListAsync();
        return studyGroupList.ToStudyGroupDto();
    }

    public async Task AddStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await studyGroupRepository.AddAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudyGroup(StudyGroupDto oldStudyGroupDto, StudyGroupDto newStudyGroupDto)
    {
        var oldStudyGroup = Schedule.CreateStudyGroup(oldStudyGroupDto.Name);
        var newStudyGroup = Schedule.CreateStudyGroup(newStudyGroupDto.Name);
        await studyGroupRepository.UpdateAsync(oldStudyGroup, newStudyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await studyGroupRepository.Delete(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }
}