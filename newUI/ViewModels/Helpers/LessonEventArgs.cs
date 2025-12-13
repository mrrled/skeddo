using System;
using Application.DtoModels;

namespace newUI.ViewModels.Helpers;

public class LessonAddedEventArgs : EventArgs
{
    public LessonDto Lesson { get; }
    public int ScheduleId { get; }
    
    public LessonAddedEventArgs(LessonDto lesson, int scheduleId)
    {
        Lesson = lesson;
        ScheduleId = scheduleId;
    }
}

public class LessonUpdatedEventArgs : EventArgs
{
    public LessonDto OldLesson { get; }
    public LessonDto NewLesson { get; }
    public int ScheduleId { get; }
    
    public LessonUpdatedEventArgs(LessonDto oldLesson, LessonDto newLesson, int scheduleId)
    {
        OldLesson = oldLesson;
        NewLesson = newLesson;
        ScheduleId = scheduleId;
    }
}

public class LessonRemovedEventArgs : EventArgs
{
    public LessonDto Lesson { get; }
    public int ScheduleId { get; }
    
    public LessonRemovedEventArgs(LessonDto lesson, int scheduleId)
    {
        Lesson = lesson;
        ScheduleId = scheduleId;
    }
}