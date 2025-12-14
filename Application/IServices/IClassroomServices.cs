using Application.DtoModels;

namespace Application.IServices;

public interface IClassroomServices
{
    public Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync();
    public Task<ClassroomDto> AddClassroom(CreateClassroomDto classroomDto);
    public Task EditClassroom(ClassroomDto classroomDto);
    public Task DeleteClassroom(ClassroomDto classroomDto);
}