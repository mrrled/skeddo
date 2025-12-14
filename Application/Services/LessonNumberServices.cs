using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class LessonNumberServices(ILessonNumberRepository lessonNumberRepository, IUnitOfWork unitOfWork) : ILessonNumberServices
{
    public async Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(Guid scheduleId)
    {
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumbersDto();
    }

    public async Task AddLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId)
    {
        var lessonNumber = LessonNumber.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await lessonNumberRepository.AddAsync(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto,
        Guid scheduleId)
    {
        var oldLessonNumber = LessonNumber.CreateLessonNumber(oldLessonNumberDto.Number, oldLessonNumberDto.Time);
        var newLessonNumber = LessonNumber.CreateLessonNumber(newLessonNumberDto.Number, newLessonNumberDto.Time);
        await lessonNumberRepository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, Guid scheduleId)
    {
        var lessonNumber = LessonNumber.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await lessonNumberRepository.Delete(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }
}