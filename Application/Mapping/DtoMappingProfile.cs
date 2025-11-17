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
            .ForMember(dest => dest.Specialty,
                opt => opt.MapFrom(src => src.SchoolSubjects.FirstOrDefault()!.Name))
            .ReverseMap();
        CreateMap<StudyGroup, DtoStudyGroup>().ReverseMap();
        CreateMap<LessonNumber, DtoLessonNumber>().ReverseMap();
    }
}