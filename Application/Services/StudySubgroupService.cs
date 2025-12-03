using Application.IServices;
using Domain.IRepositories;
using Domain.Models;

namespace Application.Services;

public class StudySubgroupService(
    IStudyGroupRepository studyGroupRepository,
    IUnitOfWork unitOfWork) : IStudySubgroupService
{
    public async Task AddStudySubgroup(StudySubgroup studySubgroup, int scheduleId)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroup.Group.Id);
        studyGroup.AddSubgroup(studySubgroup.Name);
        await studyGroupRepository.UpdateAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudySubgroup(StudySubgroup oldStudySubgroup, StudySubgroup newStudySubgroup, int scheduleId)
    {
        if (oldStudySubgroup.Group.Id != newStudySubgroup.Group.Id)
            throw new ArgumentException();
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(oldStudySubgroup.Group.Id);
        studyGroup.EditSubgroup(oldStudySubgroup.Name, newStudySubgroup.Name);
        await studyGroupRepository.UpdateAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudySubgroup(StudySubgroup studySubgroup, int scheduleId)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroup.Group.Id);
        studyGroup.DeleteSubgroup(studySubgroup.Name);
        await studyGroupRepository.UpdateAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }
}