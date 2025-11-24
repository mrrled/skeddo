using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using Domain.Models;
using Domain.IRepositories;

namespace Application.Services;

public class LessonNumberServices(ILessonNumberRepository lessonNumberRepository, IUnitOfWork unitOfWork) : ILessonNumberServices
{
    public async Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(int scheduleId)
    {
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumbersDto();
    }

    public async Task AddLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await lessonNumberRepository.AddAsync(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto,
        int scheduleId)
    {
        var oldLessonNumber = Schedule.CreateLessonNumber(oldLessonNumberDto.Number, oldLessonNumberDto.Time);
        var newLessonNumber = Schedule.CreateLessonNumber(newLessonNumberDto.Number, newLessonNumberDto.Time);
        await lessonNumberRepository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await lessonNumberRepository.Delete(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }
}