using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboMappingProfile : Profile
{
    public DboMappingProfile()
    {
        CreateMap<StudySubgroupDbo, StudySubgroup>()
            .ForMember(dest => dest.StudyGroup, opt => opt.Ignore());
        CreateMap<StudySubgroup, StudySubgroupDbo>()
            .ForMember(dest => dest.StudyGroupId, opt => opt.Ignore())
            .ForMember(dest => dest.StudyGroup, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<SchoolSubjectDbo, SchoolSubject>().ReverseMap();
        CreateMap<StudyGroup, StudyGroupDbo>()
            .ForMember(dest => dest.ScheduleGroupId, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleGroup, opt => opt.Ignore())
            .ForMember(dest => dest.Teachers, opt => opt.Ignore());
        CreateMap<StudyGroupDbo, StudyGroup>();
        CreateMap<LessonNumber, LessonNumberDbo>()
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<LessonNumberDbo, LessonNumber>();
        CreateMap<TeacherDbo, Teacher>()
            .ConstructUsing((src, ctx) => new Teacher(
                src.Id,
                src.Name, src.Surname, src.Patronymic,
                ctx.Mapper.Map<List<SchoolSubject>>(src.SchoolSubjects),
                ctx.Mapper.Map<List<StudyGroup>>(src.StudyGroups)
            ))
            .ForAllMembers(opt => opt.Ignore());
        CreateMap<Teacher, TeacherDbo>()
            .ConstructUsing((src, ctx) =>
                new TeacherDbo
                {
                    Id = src.Id,
                    Name = src.Name,
                    Surname = src.Surname,
                    Patronymic = src.Patronymic
                })
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
            .ForAllMembers(opt => opt.Ignore());
        CreateMap<Lesson, LessonDbo>()
            .ForMember(dest => dest.LessonNumberId, opt => opt.Ignore())
            .ForMember(dest => dest.LessonNumber, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.SchoolSubject, opt => opt.Ignore())
            .ForMember(dest => dest.StudyGroup, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore());
        CreateMap<LessonDraftDbo, LessonDraft>()
            .ConstructUsing((src, ctx) => new LessonDraft(
                src.Id,
                ctx.Mapper.Map<SchoolSubject>(src.SchoolSubject),
                ctx.Mapper.Map<LessonNumber>(src.LessonNumber),
                ctx.Mapper.Map<Teacher>(src.Teacher),
                ctx.Mapper.Map<StudyGroup>(src.StudyGroup),
                ctx.Mapper.Map<Classroom>(src.Classroom)
            ))
            .ForAllMembers(opt => opt.Ignore());;
        CreateMap<LessonDraft, LessonDraftDbo>()
            .ForMember(dest => dest.LessonNumberId, opt => opt.Ignore())
            .ForMember(dest => dest.LessonNumber, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.SchoolSubject, opt => opt.Ignore())
            .ForMember(dest => dest.StudyGroup, opt => opt.Ignore())
            .ForMember(dest => dest.Classroom, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore());
        CreateMap<ScheduleDbo, Schedule>();
        CreateMap<Schedule, ScheduleDbo>()
            .ForMember(dest => dest.ScheduleGroupId, opt => opt.Ignore())
            .ForMember(dest => dest.ScheduleGroup, opt => opt.Ignore())
            .ForMember(dest => dest.LessonNumbers, opt => opt.Ignore())
            .ForMember(dest => dest.LessonDrafts, opt => opt.Ignore())
            .ForMember(dest => dest.Lessons, opt => opt.Ignore());
    }
}