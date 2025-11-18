using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Classroom, DtoClassroom>().ReverseMap();
        CreateMap<Lesson, DtoLesson>().ReverseMap();
        CreateMap<Schedule, DtoSchedule>().ReverseMap();
        CreateMap<SchoolSubject, DtoSchoolSubject>().ReverseMap();
        CreateMap<Teacher, DtoTeacher>()
            .ForMember(dest => dest.SchoolSubjects,
                opt => opt.MapFrom(src => src.SchoolSubjects.Select(x => x.Name).ToList()))
            .ForMember(dest => dest.StudyGroups,
                opt => opt.MapFrom(src => src.StudyGroups.Select(x => x.Name).ToList()));
        CreateMap<StudyGroup, DtoStudyGroup>().ReverseMap();
        CreateMap<LessonNumber, DtoLessonNumber>().ReverseMap();
    }
}