using Domain.IRepositories;
using Domain.Models;
using Infrastructure.DboExtensions;
using Infrastructure.DboMapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonDraftRepository(ScheduleDbContext context) : ILessonDraftRepository
{
    public async Task AddAsync(LessonDraft lessonDraft, Guid scheduleId)
    {
        var lessonDraftDbo = lessonDraft.ToLessonDraftDbo();
        var schedule = await context.Schedules
            .Where(x => x.Id == scheduleId)
            .FirstOrDefaultAsync();
        if (schedule is null)
            throw new InvalidOperationException();
        var lessonNumber = lessonDraft.LessonNumber is null ? null :
            await context.LessonNumbers.FirstOrDefaultAsync(x =>
                x.ScheduleId == scheduleId && x.Number == lessonDraft.LessonNumber.Number);
        var studySubgroup = lessonDraft.StudySubgroup is null
            ? null
            : await context.StudySubgroups.FirstOrDefaultAsync(x =>
                x.StudyGroup.Id == lessonDraft.StudyGroup.Id && x.Name == lessonDraft.StudySubgroup.Name);
        lessonDraftDbo.StudySubgroupId = studySubgroup?.Id;
        lessonDraftDbo.ScheduleId = scheduleId;
        lessonDraftDbo.LessonNumberId = lessonNumber?.Id;
        await context.LessonDrafts.AddAsync(lessonDraftDbo);
    }

    public async Task<List<LessonDraft>> GetLessonDraftsByScheduleId(Guid scheduleId)
    {
        var lessonDraftDbos = await context.LessonDrafts.Where(x => x.ScheduleId == scheduleId)
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .ToListAsync();
        return lessonDraftDbos.ToLessonDrafts();
    }

    public async Task<LessonDraft> GetLessonDraftById(Guid id)
    {
        var lessonDraftDbo = await context.LessonDrafts
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (lessonDraftDbo is null)
            throw new InvalidOperationException();
        return lessonDraftDbo.ToLessonDraft();
    }

    public async Task Delete(LessonDraft lessonDraft)
    {
        await context.LessonDrafts.Where(x => x.Id == lessonDraft.Id).ExecuteDeleteAsync();
    }

    public async Task Update(LessonDraft lessonDraft)
    {
        var lessonDraftDbo = await context.LessonDrafts
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .FirstOrDefaultAsync(x => x.Id == lessonDraft.Id);
        if (lessonDraftDbo is null)
            throw new InvalidOperationException();
        DboMapper.Mapper.Map(lessonDraft, lessonDraftDbo);
    }
}