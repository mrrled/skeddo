using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<Classroom, DtoClassroom>();
        CreateMap<Lesson, DtoLesson>();
        CreateMap<Schedule, DtoSchedule>();
        CreateMap<SchoolSubject, DtoSchoolSubject>();
        CreateMap<Teacher, DtoTeacher>()
            .ForMember(dest => dest.Specialty,
                opt => opt.MapFrom(src => src.SchoolSubjects.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.FullName.FirstName))
            .ForMember(dest => dest.Surname,
                opt => opt.MapFrom(src => src.FullName.LastName))
            .ForMember(dest => dest.Patronymic,
                opt => opt.MapFrom(src => src.FullName.Patronymic));
        CreateMap<StudyGroup, DtoStudyGroup>();
        CreateMap<TimeSlot, DtoTimeSlot>();
    }
}