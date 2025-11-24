using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Mapping;

public static class DboMapper
{
    public static readonly IMapper Mapper =
        new MapperConfiguration(cfg => { cfg.AddProfile<DboMappingProfile>(); }, loggerFactory: new LoggerFactory())
            .CreateMapper();

    public static ClassroomDbo ToClassroomDbo(Classroom classroom)
    {
        return Mapper.Map<ClassroomDbo>(classroom);
    }

    public static List<ClassroomDbo> ToClassroomDbo(ICollection<Classroom> classrooms)
    {
        return Mapper.Map<List<ClassroomDbo>>(classrooms);
    }

    public static Classroom ToClassroom(ClassroomDbo classroom)
    {
        return Mapper.Map<Classroom>(classroom);
    }

    public static List<Classroom> ToClassroom(ICollection<ClassroomDbo> classrooms)
    {
        return Mapper.Map<List<Classroom>>(classrooms);
    }

    public static LessonDbo ToLessonDbo(Lesson lesson)
    {
        return Mapper.Map<LessonDbo>(lesson);
    }

    public static List<LessonDbo> ToLessonDbo(ICollection<Lesson> lessons)
    {
        return Mapper.Map<List<LessonDbo>>(lessons);
    }

    public static Lesson ToLesson(LessonDbo lesson)
    {
        return Mapper.Map<Lesson>(lesson);
    }

    public static List<Lesson> ToLesson(ICollection<LessonDbo> lessons)
    {
        return Mapper.Map<List<Lesson>>(lessons);
    }

    public static LessonNumberDbo ToLessonNumberDbo(LessonNumber dboLessonNumber)
    {
        return Mapper.Map<LessonNumberDbo>(dboLessonNumber);
    }

    public static List<LessonNumberDbo> ToLessonNumberDbo(ICollection<LessonNumber> dboLessonNumber)
    {
        return Mapper.Map<List<LessonNumberDbo>>(dboLessonNumber);
    }

    public static LessonNumber ToLessonNumber(LessonNumberDbo dboLessonNumber)
    {
        return Mapper.Map<LessonNumber>(dboLessonNumber);
    }

    public static List<LessonNumber> ToLessonNumber(ICollection<LessonNumberDbo> dboLessonNumber)
    {
        return Mapper.Map<List<LessonNumber>>(dboLessonNumber);
    }

    public static ScheduleDbo ToScheduleDbo(Schedule schedule)
    {
        return Mapper.Map<ScheduleDbo>(schedule);
    }

    public static List<ScheduleDbo> ToScheduleDbo(ICollection<Schedule> schedules)
    {
        return Mapper.Map<List<ScheduleDbo>>(schedules);
    }

    public static Schedule ToSchedule(ScheduleDbo schedule)
    {
        return Mapper.Map<Schedule>(schedule);
    }

    public static List<Schedule> ToSchedule(ICollection<ScheduleDbo> schedules)
    {
        return Mapper.Map<List<Schedule>>(schedules);
    }

    public static SchoolSubjectDbo ToSchoolSubjectDbo(SchoolSubject schoolSubject)
    {
        return Mapper.Map<SchoolSubjectDbo>(schoolSubject);
    }

    public static List<SchoolSubjectDbo> ToSchoolSubjectDbo(ICollection<SchoolSubject> schoolSubjects)
    {
        return Mapper.Map<List<SchoolSubjectDbo>>(schoolSubjects);
    }

    public static SchoolSubject ToSchoolSubject(SchoolSubjectDbo schoolSubject)
    {
        return Mapper.Map<SchoolSubject>(schoolSubject);
    }

    public static List<SchoolSubject> ToSchoolSubject(ICollection<SchoolSubjectDbo> schoolSubjects)
    {
        return Mapper.Map<List<SchoolSubject>>(schoolSubjects);
    }

    public static StudyGroupDbo ToStudyGroupDbo(StudyGroup studyGroup)
    {
        return Mapper.Map<StudyGroupDbo>(studyGroup);
    }

    public static List<StudyGroupDbo> ToStudyGroupDbo(ICollection<StudyGroup> studyGroups)
    {
        return Mapper.Map<List<StudyGroupDbo>>(studyGroups);
    }

    public static StudyGroup ToStudyGroup(StudyGroupDbo studyGroup)
    {
        return Mapper.Map<StudyGroup>(studyGroup);
    }

    public static List<StudyGroup> ToStudyGroup(ICollection<StudyGroupDbo> studyGroups)
    {
        return Mapper.Map<List<StudyGroup>>(studyGroups);
    }

    public static TeacherDbo ToTeacherDbo(Teacher teacher)
    {
        return Mapper.Map<TeacherDbo>(teacher);
    }

    public static List<TeacherDbo> ToTeacherDbo(ICollection<Teacher> teachers)
    {
        return Mapper.Map<List<TeacherDbo>>(teachers);
    }

    public static Teacher ToTeacher(TeacherDbo teacher)
    {
        return Mapper.Map<Teacher>(teacher);
    }

    public static List<Teacher> ToTeacher(ICollection<TeacherDbo> teachers)
    {
        return Mapper.Map<List<Teacher>>(teachers);
    }
}