using Application.DtoModels;
using Application.Extensions;
using Domain;
using Domain.Models;

namespace Application.Services;

public class ScheduleServices(IScheduleRepository repository, IUnitOfWork unitOfWork) : IScheduleServices
{
    public async Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await repository.GetScheduleListAsync();
        return scheduleList.ToScheduleDto();
    }

    public async Task AddSchedule(ScheduleDto scheduleDto)
    {
        var schedule = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await repository.AddAsync(schedule);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchedule(ScheduleDto oldScheduleDto, ScheduleDto newScheduleDto)
    {
        var oldSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        var newSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        await repository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchedule(ScheduleDto scheduleDto)
    {
        var schoolSubject = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await repository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}