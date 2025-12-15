using Application.DtoModels;
using Domain.Models;

namespace Application.IServices;

public interface IStudySubgroupService
{
    public Task AddStudySubgroup(StudySubgroupDto studySubgroup);
    public Task EditStudySubgroup(StudySubgroupDto oldStudySubgroup, StudySubgroupDto newStudySubgroup);
    public Task DeleteStudySubgroup(StudySubgroupDto studySubgroup);
}