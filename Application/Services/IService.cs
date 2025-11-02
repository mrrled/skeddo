using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.Services;

public interface IService
{
    public List<DtoClassroom> FetchClassroomsFromBackend();
    public List<DtoLesson> FetchLessonsFromBackend();
    public List<DtoSchedule> FetchSchedulesFromBackend();
    public List<DtoSchoolSubject> FetchSchoolSubjectsFromBackend();
    public List<DtoStudyGroup> FetchStudyGroupsFromBackend();
    public List<DtoTeacher> FetchTeachersFromBackend();
    public List<DtoTimeSlot> FetchTimeSlotsFromBackend();
}