using Application.DtoModels;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Mapping;

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
    
    public static Classroom ToClassroom(ClassroomDto classroom)
    {
        return Mapper.Map<Classroom>(classroom);
    }
    
    public static List<Classroom> ToClassroom(ICollection<ClassroomDto> classrooms)
    {
        return Mapper.Map<List<Classroom>>(classrooms);
    }
    
    public static LessonDto ToLessonDto(Lesson lesson)
    {
        return Mapper.Map<LessonDto>(lesson);
    }
    
    public static List<LessonDto> ToLessonDto(ICollection<Lesson> lessons)
    {
        return Mapper.Map<List<LessonDto>>(lessons);
    }
    
    public static Lesson ToLesson(LessonDto lesson)
    {
        return Mapper.Map<Lesson>(lesson);
    }
    
    public static List<Lesson> ToLesson(ICollection<LessonDto> lessons)
    {
        return Mapper.Map<List<Lesson>>(lessons);
    }
    
    public static LessonNumberDto ToLessonNumberDto(LessonNumber dtoLessonNumber)
    {
        return Mapper.Map<LessonNumberDto>(dtoLessonNumber);
    }
    public static List<LessonNumberDto> ToLessonNumberDto(ICollection<LessonNumber> dtoLessonNumber)
    {
        return Mapper.Map<List<LessonNumberDto>>(dtoLessonNumber);
    }
    
    public static LessonNumber ToLessonNumber(LessonNumberDto lessonNumberDto)
    {
        return Mapper.Map<LessonNumber>(lessonNumberDto);
    }

    public static List<LessonNumber> ToLessonNumber(ICollection<LessonNumberDto> dtoLessonNumber)
    {
        return Mapper.Map<List<LessonNumber>>(dtoLessonNumber);
    }
    
    public static ScheduleDto ToScheduleDto(Schedule schedule)
    {
        return Mapper.Map<ScheduleDto>(schedule);
    }
    
    public static List<ScheduleDto> ToScheduleDto(ICollection<Schedule> schedules)
    {
        return Mapper.Map<List<ScheduleDto>>(schedules);
    }
    
    public static Schedule ToSchedule(ScheduleDto schedule)
    {
        return Mapper.Map<Schedule>(schedule);
    }
    
    public static List<Schedule> ToSchedule(ICollection<ScheduleDto> schedules)
    {
        return Mapper.Map<List<Schedule>>(schedules);
    }
    
    public static SchoolSubjectDto ToSchoolSubjectDto(SchoolSubject schoolSubject)
    {
        return Mapper.Map<SchoolSubjectDto>(schoolSubject);
    }
    
    public static List<SchoolSubjectDto> ToSchoolSubjectDto(ICollection<SchoolSubject> schoolSubjects)
    {
        return Mapper.Map<List<SchoolSubjectDto>>(schoolSubjects);
    }
    
    public static SchoolSubject ToSchoolSubject(SchoolSubjectDto schoolSubject)
    {
        return Mapper.Map<SchoolSubject>(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(ICollection<SchoolSubjectDto> schoolSubjects)
    {
        return Mapper.Map<List<SchoolSubject>>(schoolSubjects);
    }
    
    public static StudyGroupDto ToStudyGroupDto(StudyGroup studyGroup)
    {
        return Mapper.Map<StudyGroupDto>(studyGroup);
    }
    
    public static List<StudyGroupDto> ToStudyGroupDto(ICollection<StudyGroup> studyGroups)
    {
        return Mapper.Map<List<StudyGroupDto>>(studyGroups);
    }
    public static StudyGroup ToStudyGroup(StudyGroupDto studyGroup)
    {
        return Mapper.Map<StudyGroup>(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(ICollection<StudyGroupDto> studyGroups)
    {
        return Mapper.Map<List<StudyGroup>>(studyGroups);
    }
    
    public static TeacherDto ToTeacherDto(Teacher teacher)
    {
        return Mapper.Map<TeacherDto>(teacher);
    }
    
    public static List<TeacherDto> ToTeacherDto(ICollection<Teacher> teachers)
    {
        return Mapper.Map<List<TeacherDto>>(teachers);
    }
    
    public static Teacher ToTeacher(TeacherDto teacher)
    {
        return Mapper.Map<Teacher>(teacher);
    }
    
    public static List<Teacher> ToTeacher(ICollection<TeacherDto> teachers)
    {
        return Mapper.Map<List<Teacher>>(teachers);
    }
}