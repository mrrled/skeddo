using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Mapping;

public class DboMappingProfile : Profile
{
    public DboMappingProfile()
    {
        CreateMap<SchoolSubjectDbo, SchoolSubject>();
        CreateMap<StudyGroupDbo, StudyGroup>();
        CreateMap<TeacherDbo, Teacher>()
            .ConstructUsing((src, ctx) => new Teacher(
                src.Id,
                new FullName(src.FirstName, src.LastName, src.Patronymic),
                ctx.Mapper.Map<List<SchoolSubject>>(src.SchoolSubjects),
                ctx.Mapper.Map<List<StudyGroup>>(src.StudyGroups)
            ))
            .ForAllMembers(opt => opt.Ignore());;
        CreateMap<ClassroomDbo, Classroom>();
        CreateMap<LessonDbo, Lesson>()
            .ConstructUsing((src, ctx) => new Lesson(
                src.Id,
                ctx.Mapper.Map<SchoolSubject>(src.SchoolSubject),
                new LessonNumber(src.LessonNumber),
                ctx.Mapper.Map<Teacher>(src.Teacher),
                ctx.Mapper.Map<StudyGroup>(src.StudyGroup),
                ctx.Mapper.Map<Classroom>(src.Classroom)
            ))
            .ForAllMembers(opt => opt.Ignore());
    }
}