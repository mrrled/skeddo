using Application.UIModels;
using Domain;

namespace Application.Converters;

public class TeacherConverter : IConverter<Domain.Models.Teacher, Teacher>
{
    public static Teacher Convert(Domain.Models.Teacher teacher)
    {
        return new Teacher(teacher.Name, teacher.Surname, teacher.MiddleName,
            teacher.Specializations.Select(spec => spec.Name).FirstOrDefault()?.ToString());
    }
}