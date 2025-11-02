using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboLessonProfile : Profile
{
    public DboLessonProfile()
    {
        CreateMap<DboLesson, Lesson>()
            .ConstructUsing((src, context) =>
            {
                var teachers = context.Items["Teachers"] as Dictionary<int, Teacher>;
                var subjects = context.Items["Subjects"] as Dictionary<string, SchoolSubject>;
                var groups = context.Items["Groups"] as Dictionary<string, StudyGroup>;
                var classrooms = context.Items["Classrooms"] as Dictionary<string, Classroom>;
                var timeSlots = context.Items["TimeSlots"] as Dictionary<int, TimeSlot>;
                
                return new Lesson(
                    src.Id,
                    subjects![src.SchoolSubject],
                    timeSlots![src.LessonNumber],
                    teachers![src.TeacherId],
                    groups![src.StudyGroup],
                    classrooms![src.Classroom],
                    string.Empty,
                    false
                );
            });
    }
}