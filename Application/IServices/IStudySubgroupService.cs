using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.IServices;

public interface IStudySubgroupService
{
    public Task<Result> AddStudySubgroup(StudySubgroupDto studySubgroup);
    public Task<Result> EditStudySubgroup(StudySubgroupDto oldStudySubgroup, StudySubgroupDto newStudySubgroup);
    public Task<Result> DeleteStudySubgroup(StudySubgroupDto studySubgroup);
}