using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.DtoMapping;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Classroom, ClassroomDto>();
        CreateMap<Lesson, LessonDto>();
        CreateMap<Schedule, ScheduleDto>();
        CreateMap<SchoolSubject, SchoolSubjectDto>();
        CreateMap<Teacher, TeacherDto>();
        CreateMap<StudyGroup, StudyGroupDto>();
        CreateMap<LessonNumber, LessonNumberDto>();
        CreateMap<LessonDraft, LessonDraftDto>();
        CreateMap<StudySubgroup, StudySubgroupDto>();
    }
}