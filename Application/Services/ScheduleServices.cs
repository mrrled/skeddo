using Application.DtoModels;
using Application.DtoExtensions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class ScheduleServices(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork) : IScheduleServices
{
    public async Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await scheduleRepository.GetScheduleListAsync();
        return scheduleList.ToSchedulesDto();
    }

    public async Task AddSchedule(ScheduleDto scheduleDto)
    {
        var schedule = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await scheduleRepository.AddAsync(schedule);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchedule(ScheduleDto oldScheduleDto, ScheduleDto newScheduleDto)
    {
        var oldSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        var newSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        await scheduleRepository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchedule(ScheduleDto scheduleDto)
    {
        var schoolSubject = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await scheduleRepository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}