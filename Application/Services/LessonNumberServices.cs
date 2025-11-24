using Application.DtoModels;
using Application.Extensions;
using AutoMapper;
using Domain;
using Domain.Models;

namespace Application.Services;

public class LessonNumberServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : ILessonNumberServices
{
    public async Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(int scheduleId)
    {
        var lessonNumbers = await repository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumberDto();
    }

    public async Task AddLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await repository.AddAsync(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto,
        int scheduleId)
    {
        var oldLessonNumber = Schedule.CreateLessonNumber(oldLessonNumberDto.Number, oldLessonNumberDto.Time);
        var newLessonNumber = Schedule.CreateLessonNumber(newLessonNumberDto.Number, newLessonNumberDto.Time);
        await repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await repository.Delete(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }
}