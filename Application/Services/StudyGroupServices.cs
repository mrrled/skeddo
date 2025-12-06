using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class StudyGroupServices(IStudyGroupRepository studyGroupRepository, IUnitOfWork unitOfWork) : IStudyGroupServices
{
    public async Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await studyGroupRepository.GetStudyGroupListAsync(1);
        return studyGroupList.ToStudyGroupsDto();
    }

    public async Task AddStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = StudyGroup.CreateStudyGroup(studyGroupDto.Id, studyGroupDto.Name);
        await studyGroupRepository.AddAsync(studyGroup, 1);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studyGroupDto.Id);
        if (studyGroup is null)
            throw new  ArgumentException($"StudyGroup with id {studyGroupDto.Id} not found");
        if (studyGroupDto.Name != studyGroup.Name)
            studyGroup.SetName(studyGroupDto.Name);
        await studyGroupRepository.UpdateAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studyGroupDto.Id);
        if (studyGroup is null)
            throw new  ArgumentException($"StudyGroup with id {studyGroupDto.Id} not found");
        await studyGroupRepository.Delete(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }
}