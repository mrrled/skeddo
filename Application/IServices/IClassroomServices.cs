using Application.DtoModels;
using Domain;

namespace Application.IServices;

public interface IClassroomServices
{
    public Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync();
    public Task<Result<ClassroomDto>> AddClassroom(CreateClassroomDto classroomDto);
    public Task<Result> EditClassroom(ClassroomDto classroomDto);
    public Task<Result> DeleteClassroom(ClassroomDto classroomDto);
}