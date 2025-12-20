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
            throw new InvalidOperationException($"Schedule with id {scheduleId} not found.");
        var lessonNumber = lessonDraft.LessonNumber is null
            ? null
            : await context.LessonNumbers.FirstOrDefaultAsync(x =>
                x.ScheduleId == scheduleId && x.Number == lessonDraft.LessonNumber.Number);
        var studySubgroup = lessonDraft.StudySubgroup is null || lessonDraft.StudyGroup is null
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
            .Include(x => x.StudySubgroup)
            .ToListAsync();
        return lessonDraftDbos.ToLessonDrafts();
    }

    public async Task<LessonDraft?> GetLessonDraftById(Guid id)
    {
        var lessonDraftDbo = await context.LessonDrafts
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .ThenInclude(x => x.StudySubgroups)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Include(x => x.StudySubgroup)
            .FirstOrDefaultAsync(x => x.Id == id);
        return lessonDraftDbo?.ToLessonDraft();
    }

    public async Task Delete(LessonDraft lessonDraft)
    {
        var dbo = await context.LessonDrafts.FirstAsync(x => x.Id == lessonDraft.Id);
        context.LessonDrafts.Remove(dbo);
    }

    public async Task Update(LessonDraft lessonDraft)
    {
        var lessonDraftDbo = await context.LessonDrafts
            .Include(x => x.Teacher)
            .Include(x => x.Classroom)
            .Include(x => x.StudyGroup)
            .Include(x => x.SchoolSubject)
            .Include(x => x.LessonNumber)
            .Include(x => x.StudySubgroup)
            .FirstOrDefaultAsync(x => x.Id == lessonDraft.Id);
        if (lessonDraftDbo is null)
            throw new InvalidOperationException($"Lesson draft with id {lessonDraft.Id} not found.");
        DboMapper.Mapper.Map(lessonDraft, lessonDraftDbo);
        if (lessonDraft.LessonNumber != null)
        {
            var lessonNumberDbo = await context.LessonNumbers
                .FirstOrDefaultAsync(x => x.ScheduleId == lessonDraft.ScheduleId
                                          && x.Number == lessonDraft.LessonNumber.Number);
            lessonDraftDbo.LessonNumberId = lessonNumberDbo?.Id;
        }
        else
        {
            lessonDraftDbo.LessonNumberId = null;
        }

        if (lessonDraft.StudySubgroup != null && lessonDraft.StudyGroup != null)
        {
            var studySubgroupDbo = await context.StudySubgroups
                .FirstOrDefaultAsync(x => x.StudyGroupId == lessonDraft.StudyGroup.Id
                                          && x.Name == lessonDraft.StudySubgroup.Name);
            lessonDraftDbo.StudySubgroupId = studySubgroupDbo?.Id;
        }
        else
        {
            lessonDraftDbo.StudySubgroupId = null;
        }
    }
}