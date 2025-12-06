using Application.DtoModels;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.DtoMapping;

public static class DtoMapper
{
    
    public static readonly IMapper Mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<DtoMappingProfile>();
    }, loggerFactory: new LoggerFactory())
        .CreateMapper();
    
    public static ClassroomDto ToClassroomDto(Classroom classroom)
    {
        return Mapper.Map<ClassroomDto>(classroom);
    }
    
    public static List<ClassroomDto> ToClassroomDto(ICollection<Classroom> classrooms)
    {
        return Mapper.Map<List<ClassroomDto>>(classrooms);
    }
    
    public static LessonDto ToLessonDto(Lesson lesson)
    {
        return Mapper.Map<LessonDto>(lesson);
    }
    
    public static List<LessonDto> ToLessonDto(ICollection<Lesson> lessons)
    {
        return Mapper.Map<List<LessonDto>>(lessons);
    }
    
    public static LessonNumberDto ToLessonNumberDto(LessonNumber dtoLessonNumber)
    {
        return Mapper.Map<LessonNumberDto>(dtoLessonNumber);
    }
    public static List<LessonNumberDto> ToLessonNumberDto(ICollection<LessonNumber> dtoLessonNumber)
    {
        return Mapper.Map<List<LessonNumberDto>>(dtoLessonNumber);
    }
    
    public static ScheduleDto ToScheduleDto(Schedule schedule)
    {
        return Mapper.Map<ScheduleDto>(schedule);
    }
    
    public static List<ScheduleDto> ToScheduleDto(ICollection<Schedule> schedules)
    {
        return Mapper.Map<List<ScheduleDto>>(schedules);
    }
    
    public static SchoolSubjectDto ToSchoolSubjectDto(SchoolSubject schoolSubject)
    {
        return Mapper.Map<SchoolSubjectDto>(schoolSubject);
    }
    
    public static List<SchoolSubjectDto> ToSchoolSubjectDto(ICollection<SchoolSubject> schoolSubjects)
    {
        return Mapper.Map<List<SchoolSubjectDto>>(schoolSubjects);
    }
    
    public static StudyGroupDto ToStudyGroupDto(StudyGroup studyGroup)
    {
        return Mapper.Map<StudyGroupDto>(studyGroup);
    }
    
    public static List<StudyGroupDto> ToStudyGroupDto(ICollection<StudyGroup> studyGroups)
    {
        return Mapper.Map<List<StudyGroupDto>>(studyGroups);
    }
    
    public static TeacherDto ToTeacherDto(Teacher teacher)
    {
        return Mapper.Map<TeacherDto>(teacher);
    }
    
    public static List<TeacherDto> ToTeacherDto(ICollection<Teacher> teachers)
    {
        return Mapper.Map<List<TeacherDto>>(teachers);
    }

    public static LessonDraftDto ToLessonDraftDto(LessonDraft lesson)
    {
        return Mapper.Map<LessonDraftDto>(lesson);
    }

    public static List<LessonDraftDto> ToLessonDraftDto(ICollection<LessonDraft> lessons)
    {
        return Mapper.Map<List<LessonDraftDto>>(lessons);
    }
}