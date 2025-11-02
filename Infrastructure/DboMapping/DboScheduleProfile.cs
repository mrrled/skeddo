using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboScheduleProfile : Profile
{
    public DboScheduleProfile()
    {
        CreateMap<DboSchedule, Schedule>()
            .ConstructUsing((src, context) =>
            {
                var lessonsBySchedule = context.Items["LessonsBySchedule"]
                    as Dictionary<int, HashSet<Lesson>>;

                var lessons = lessonsBySchedule?.GetValueOrDefault(src.Id)
                              ?? new HashSet<Lesson>();

                return new Schedule(src.Id, lessons);
            });
    }
}