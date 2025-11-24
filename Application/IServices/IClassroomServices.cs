using Application.DtoModels;

namespace Application.IServices;

public interface IClassroomServices
{
    public Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync();
    public Task AddClassroom(ClassroomDto classroomDto);
    public Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto);
    public Task DeleteClassroom(ClassroomDto classroomDto);
}