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
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.Classrooms.ToClassroom(mapper);
    }

    public async Task<List<SchoolSubject>> GetSchoolSubjectListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.SchoolSubjects)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.SchoolSubjects.ToSchoolSubject(mapper);
    }

    public async Task<List<StudyGroup>> GetStudyGroupListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.StudyGroups.ToStudyGroup(mapper);
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

    public async Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(x => x.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        return schedule.LessonNumbers.ToLessonNumber(mapper);
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

    public async Task<List<Schedule>> GetScheduleListAsync()
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        return scheduleGroup.Schedules.ToSchedule(mapper);
    }

    public async Task<Lesson> GetLessonByIdAsync(int id, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lesson = schedule.Lessons.FirstOrDefault(x => x.Id == id);
        if (lesson is null)
            throw new NullReferenceException();
        return lesson.ToLesson(mapper);
    }

    public async Task<Teacher> GetTeacherByIdAsync(int id)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Teachers)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var teacher = scheduleGroup.Teachers.FirstOrDefault(x => x.Id == id);
        if (teacher is null)
            throw new NullReferenceException();
        return teacher.ToTeacher(mapper);
    }

    public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schedule = scheduleGroup.Schedules.FirstOrDefault(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        return schedule.ToSchedule(mapper);
    }

    public async Task AddAsync(Teacher teacher)
    {
        var teacherDbo = teacher.ToTeacherDbo(mapper);
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.Teachers.Add(teacherDbo);
    }

    public async Task AddAsync(Classroom classroom)
    {
        var classroomDbo = classroom.ToClassroomDbo(mapper);
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.Classrooms.Add(classroomDbo);
    }

    public async Task AddAsync(StudyGroup studyGroup)
    {
        var studyGroupDbo = studyGroup.ToStudyGroupDbo(mapper);
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.StudyGroups.Add(studyGroupDbo);
    }

    public async Task AddAsync(SchoolSubject schoolSubject)
    {
        var schoolSubjectDbo = schoolSubject.ToSchoolSubjectDbo(mapper);
        var scheduleGroup = await context.ScheduleGroups.FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        scheduleGroup.SchoolSubjects.Add(schoolSubjectDbo);
    }

    public async Task AddAsync(Lesson lesson, int scheduleId)
    {
        var lessonDbo = lesson.ToLessonDbo(mapper);
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new NullReferenceException();
        schedule.Lessons.Add(lessonDbo);
    }

    public async Task AddAsync(LessonNumber lessonNumber, int scheduleId)
    {
        var lessonNumberDbo = lessonNumber.ToLessonNumberDbo(mapper);
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new NullReferenceException();
        schedule.LessonNumbers.Add(lessonNumberDbo);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        var teacherDbo = await context.Teachers.FirstOrDefaultAsync(x => x.Id == teacher.Id);
        if (teacherDbo is null)
            throw new ArgumentException();
        mapper.Map(teacher, teacherDbo);
    }

    public async Task UpdateAsync(Classroom oldClassroom, Classroom newClassroom)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var classroomDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == oldClassroom.Name);
        if (classroomDbo is null)
            throw new NullReferenceException();
        mapper.Map(newClassroom, classroomDbo);
    }

    public async Task UpdateAsync(StudyGroup oldStudyGroup, StudyGroup newStudyGroup)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var studyGroupDbo = scheduleGroup.StudyGroups.FirstOrDefault(x => x.Name == oldStudyGroup.Name);
        if (studyGroupDbo is null)
            throw new NullReferenceException();
        mapper.Map(newStudyGroup, studyGroupDbo);
    }

    public async Task UpdateAsync(SchoolSubject oldSchoolSubject, SchoolSubject newSchoolSubject)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schoolSubjectDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == oldSchoolSubject.Name);
        if (schoolSubjectDbo is null)
            throw new NullReferenceException();
        mapper.Map(newSchoolSubject, schoolSubjectDbo);
    }

    public async Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonNumberDbo = schedule.LessonNumbers.FirstOrDefault(x => x.Number == oldLessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new NullReferenceException();
        mapper.Map(newLessonNumber, lessonNumberDbo);
    }

    public async Task Delete(Teacher teacher)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Teachers)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var teacherDbo = scheduleGroup.Teachers.FirstOrDefault(x => x.Id == teacher.Id);
        if (teacherDbo is null)
            throw new NullReferenceException();
        scheduleGroup.Teachers.Remove(teacherDbo);
    }

    public async Task Delete(Classroom classroom)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.Classrooms)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var classroomDbo = scheduleGroup.Classrooms.FirstOrDefault(x => x.Name == classroom.Name);
        if (classroomDbo is null)
            throw new NullReferenceException();
        scheduleGroup.Classrooms.Remove(classroomDbo);
    }

    public async Task Delete(StudyGroup studyGroup)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.StudyGroups)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var studyGroupDbo = scheduleGroup.StudyGroups.FirstOrDefault(x => x.Name == studyGroup.Name);
        if (studyGroupDbo is null)
            throw new NullReferenceException();
        scheduleGroup.StudyGroups.Remove(studyGroupDbo);
    }

    public async Task Delete(SchoolSubject schoolSubject)
    {
        var scheduleGroup = await context.ScheduleGroups
            .Include(scheduleGroupDbo => scheduleGroupDbo.SchoolSubjects)
            .FirstOrDefaultAsync();
        if (scheduleGroup is null)
            throw new NullReferenceException();
        var schoolSubjectDbo = scheduleGroup.SchoolSubjects.FirstOrDefault(x => x.Name == schoolSubject.Name);
        if (schoolSubjectDbo is null)
            throw new NullReferenceException();
        scheduleGroup.SchoolSubjects.Remove(schoolSubjectDbo);
    }

    public async Task Delete(LessonNumber lessonNumber, int scheduleId)
    {
        var schedule = await context.Schedules
            .Include(scheduleDbo => scheduleDbo.LessonNumbers)
            .FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (schedule is null)
            throw new NullReferenceException();
        var lessonNumberDbo = schedule.LessonNumbers.FirstOrDefault(x => x.Number == lessonNumber.Number);
        if (lessonNumberDbo is null)
            throw new NullReferenceException();
        schedule.LessonNumbers.Remove(lessonNumberDbo);
    }
}