using AutoMapper;
using Domain;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScheduleRepository(ScheduleDbContext context, IMapper mapper) : IScheduleRepository
{
    public async Task<List<Classroom>> GetClassroomListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Lesson>> GetLessonListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Schedule>> GetScheduleListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<SchoolSubject>> GetSchoolSubjectListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<StudyGroup>> GetStudyGroupListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Teacher>> GetTeacherListAsync()
    {
        var teachers = await context.Teachers
            .Where(x => x.ScheduleGroupId == 1)
            .Include(teacherDbo => teacherDbo.SchoolSubjects)
            .Include(teacherDbo => teacherDbo.StudyGroups)
            .ToListAsync();

        return teachers.ToTeacher(mapper);
    }

    public Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task<Lesson> GetLessonByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Teacher> GetTeacherByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Schedule> GetScheduleByIdAsync(int scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Classroom classroom)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(StudyGroup studyGroup)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(SchoolSubject schoolSubject)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(LessonNumber lessonNumber, int scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Classroom oldClassroom, Classroom newClassroom)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(StudyGroup oldStudyGroup, StudyGroup newStudyGroup)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(SchoolSubject oldSchoolSubject, SchoolSubject newSchoolSubject)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Classroom classroom)
    {
        throw new NotImplementedException();
    }

    public Task Delete(StudyGroup studyGroup)
    {
        throw new NotImplementedException();
    }

    public Task Delete(SchoolSubject schoolSubject)
    {
        throw new NotImplementedException();
    }

    public Task Delete(LessonNumber lessonNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Lesson>> GetLessonsByScheduleIdAsync(int scheduleId)
    {
        var lessons = await context.Lessons.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .ToListAsync();
        return lessons.ToLesson(mapper);
    }

    public async Task AddTeacherAsync(Teacher teacher)
    {
        var scheduleGroupId = 1;
        var dbo = teacher.ToTeacherDbo(mapper);
        await context.Teachers.AddAsync(dbo);
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync(x => x.Id == 1);
        if (scheduleGroup is null)
            throw new ArgumentException($"Schedule group with {scheduleGroupId} id not found");
        scheduleGroup.Teachers.Add(dbo);
        await context.SaveChangesAsync();
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        throw new NotImplementedException();
    }

    public async Task AddClassroomAsync(Classroom classroom)
    {
        throw new NotImplementedException();
    }

    public async Task AddStudyGroupAsync(StudyGroup studyGroup)
    {
        throw new NotImplementedException();
    }

    public void Update(Lesson lesson)
    {
        var lessonDbo = context.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        mapper.Map(lesson, lessonDbo);
        //_unitOfWork.SaveChangesAsync() в Application
    }

    public void UpdateClassroom(Lesson lesson, Classroom classroom)
    {
        var lessonDbo = context.Lessons.FirstOrDefault(x => x.Id == lesson.Id);
        if (lessonDbo is null)
            throw new ArgumentException($"Lesson id {lesson.Id} not found");
        lessonDbo.Classroom = mapper.Map<ClassroomDbo>(classroom);
        //_unitOfWork.SaveChangesAsync() в Application
    }
}