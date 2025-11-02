using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboTeacherProfile : Profile
{
    public DboTeacherProfile()
    {
        CreateMap<DboTeacher, Teacher>()
            .ConstructUsing((src, context) =>
            {
                // Разбиваем FullName на части
                var nameParts = src.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var surname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                var name = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                var patronymic = nameParts.Length > 2 ? nameParts[2] : string.Empty;
                
                // Получаем связанные данные
                var specializationsByTeacher = context.Items["SpecializationsByTeacher"] 
                    as Dictionary<int, List<SchoolSubject>>;
                var studyGroupsByTeacher = context.Items["StudyGroupsByTeacher"] 
                    as Dictionary<int, List<StudyGroup>>;
                
                var specializations = specializationsByTeacher?.GetValueOrDefault(src.Id) 
                                      ?? new List<SchoolSubject>();
                var studyGroups = studyGroupsByTeacher?.GetValueOrDefault(src.Id) 
                                  ?? new List<StudyGroup>();
                
                return new Teacher(
                    src.Id,
                    name,
                    surname,
                    patronymic,
                    specializations,
                    studyGroups,
                    src.Description
                );
            });
    }
}