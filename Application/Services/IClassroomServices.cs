using Application.DtoModels;

namespace Application.Services;

public interface IClassroomServices
{
    public Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync();
    public Task AddClassroom(ClassroomDto classroomDto);
    public Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto);
    public Task DeleteClassroom(ClassroomDto classroomDto);
}