using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Classroom, ClassroomDto>().ReverseMap();
        CreateMap<Lesson, LessonDto>().ReverseMap();
        CreateMap<Schedule, ScheduleDto>().ReverseMap();
        CreateMap<SchoolSubject, SchoolSubjectDto>().ReverseMap();
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.SchoolSubjects,
                opt => opt.MapFrom(src => src.SchoolSubjects.Select(x => x.Name).ToList()))
            .ForMember(dest => dest.StudyGroups,
                opt => opt.MapFrom(src => src.StudyGroups.Select(x => x.Name).ToList()));
        CreateMap<StudyGroup, StudyGroupDto>().ReverseMap();
        CreateMap<LessonNumber, LessonNumberDto>().ReverseMap();
    }
}