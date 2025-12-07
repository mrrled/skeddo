using Application.DtoModels;
using Application.IServices;
using Domain.IRepositories;
using Domain.Models;

namespace Application.Services;

public class StudySubgroupService(
    IStudyGroupRepository studyGroupRepository,
    IStudySubgroupRepository studySubgroupRepository,
    IUnitOfWork unitOfWork) : IStudySubgroupService
{
    public async Task AddStudySubgroup(StudySubgroupDto studySubgroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id);
        if  (studyGroup is null)
            throw new ArgumentNullException();
        var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name);
        await studySubgroupRepository.AddAsync(studySubgroup, studyGroup.Id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudySubgroup(StudySubgroupDto oldStudySubgroupDto, StudySubgroupDto newStudySubgroupDto)
    {
        if (oldStudySubgroupDto.StudyGroup.Id != newStudySubgroupDto.StudyGroup.Id)
            throw new ArgumentException();
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(oldStudySubgroupDto.StudyGroup.Id);
        if  (studyGroup is null)
            throw new ArgumentNullException();
        var oldStudySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, oldStudySubgroupDto.Name);
        var newStudySubgroup =  StudySubgroup.CreateStudySubgroup(studyGroup, newStudySubgroupDto.Name);
        await studySubgroupRepository.UpdateAsync(oldStudySubgroup, newStudySubgroup, studyGroup.Id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudySubgroup(StudySubgroupDto studySubgroupDto)
    {
        var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(studySubgroupDto.StudyGroup.Id);
        if  (studyGroup is null)
            throw new ArgumentNullException();
        var studySubgroup = StudySubgroup.CreateStudySubgroup(studyGroup, studySubgroupDto.Name);
        await studySubgroupRepository.Delete(studySubgroup, studyGroup.Id);
        await unitOfWork.SaveChangesAsync();
    }
}