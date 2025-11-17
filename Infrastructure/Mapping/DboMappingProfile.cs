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
        CreateMap<LessonNumberDbo, LessonNumber>().ReverseMap();
        CreateMap<TeacherDbo, Teacher>()
            .ConstructUsing((src, ctx) => new Teacher(
                src.Id,
                src.Name, src.Surname, src.Patronymic,
                ctx.Mapper.Map<List<SchoolSubject>>(src.SchoolSubjects),
                ctx.Mapper.Map<List<StudyGroup>>(src.StudyGroups)
            ))
            .ReverseMap()
            .ForAllMembers(opt => opt.Ignore());
        CreateMap<ClassroomDbo, Classroom>().ReverseMap();
        CreateMap<LessonDbo, Lesson>()
            .ConstructUsing((src, ctx) => new Lesson(
                src.Id,
                ctx.Mapper.Map<SchoolSubject>(src.SchoolSubject),
                ctx.Mapper.Map<LessonNumber>(src.LessonNumber),
                ctx.Mapper.Map<Teacher>(src.Teacher),
                ctx.Mapper.Map<StudyGroup>(src.StudyGroup),
                ctx.Mapper.Map<Classroom>(src.Classroom)
            ))
            .ReverseMap();
        CreateMap<Lesson, LessonDbo>().ReverseMap();
        CreateMap<Teacher, TeacherDbo>().ReverseMap();
    }
}