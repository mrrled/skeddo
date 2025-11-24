using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class StudyGroupDtoExtensions
{
    public static StudyGroupDto ToStudyGroupDto(this StudyGroup studyGroup)
    {
        return DtoMapper.ToStudyGroupDto(studyGroup);
    }
    
    public static List<StudyGroupDto> ToStudyGroupsDto(this ICollection<StudyGroup> studyGroups)
    {
        return DtoMapper.ToStudyGroupDto(studyGroups);
    }
    public static StudyGroup ToStudyGroup(this StudyGroupDto studyGroup)
    {
        return DtoMapper.ToStudyGroup(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroups(this ICollection<StudyGroupDto> studyGroups)
    {
        return DtoMapper.ToStudyGroup(studyGroups);
    }
}