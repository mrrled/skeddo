using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class StudyGroupServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : IStudyGroupServices
{
    public async Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await repository.GetStudyGroupListAsync();
        return studyGroupList.ToStudyGroupDto();
    }

    public async Task AddStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.AddAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudyGroup(StudyGroupDto oldStudyGroupDto, StudyGroupDto newStudyGroupDto)
    {
        var oldStudyGroup = Schedule.CreateStudyGroup(oldStudyGroupDto.Name);
        var newStudyGroup = Schedule.CreateStudyGroup(newStudyGroupDto.Name);
        await repository.UpdateAsync(oldStudyGroup, newStudyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.Delete(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }
}