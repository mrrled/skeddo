using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class StudyGroupExtensions
{
    public static StudyGroupDto ToStudyGroupDto(this StudyGroup studyGroup)
    {
        return DtoMapper.ToStudyGroupDto(studyGroup);
    }
    
    public static List<StudyGroupDto> ToStudyGroupDto(this ICollection<StudyGroup> studyGroups)
    {
        return DtoMapper.ToStudyGroupDto(studyGroups);
    }
    public static StudyGroup ToStudyGroup(this StudyGroupDto studyGroup)
    {
        return DtoMapper.ToStudyGroup(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(this ICollection<StudyGroupDto> studyGroups)
    {
        return DtoMapper.ToStudyGroup(studyGroups);
    }
}