using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Mapping;

public class DboMappingProfile : Profile
{
    public DboMappingProfile()
    {
        CreateMap<SchoolSubjectDbo, SchoolSubject>().ReverseMap();
        CreateMap<StudyGroupDbo, StudyGroup>().ReverseMap();
        CreateMap<TeacherDbo, Teacher>()
            .ConstructUsing((src, ctx) => new Teacher(
                src.Id,
                new FullName(src.FirstName, src.LastName, src.Patronymic),
                ctx.Mapper.Map<List<SchoolSubject>>(src.SchoolSubjects),
                ctx.Mapper.Map<List<StudyGroup>>(src.StudyGroups)
            ))
            .ForAllMembers(opt => opt.Ignore());;
        CreateMap<ClassroomDbo, Classroom>().ReverseMap();
        CreateMap<LessonDbo, Lesson>()
            .ConstructUsing((src, ctx) => new Lesson(
                src.Id,
                ctx.Mapper.Map<SchoolSubject>(src.SchoolSubject),
                new LessonNumber(src.LessonNumber),
                ctx.Mapper.Map<Teacher>(src.Teacher),
                ctx.Mapper.Map<StudyGroup>(src.StudyGroup),
                ctx.Mapper.Map<Classroom>(src.Classroom)
            ));
        CreateMap<Lesson, LessonDbo>();
        CreateMap<Teacher, TeacherDbo>()
            .ForMember(x => x.FirstName,
                opt => opt.MapFrom(t => t.FullName.FirstName))
            .ForMember(x => x.LastName,
                opt => opt.MapFrom(t => t.FullName.LastName))
            .ForMember(x => x.Patronymic,
                opt => opt.MapFrom(t => t.FullName.Patronymic));
    }
}