namespace Domain.Models;

public record Teacher(
    string Name,
    string Surname,
    string MiddleName,
    SchoolSubject[] Specializations,
    StudyGroup[] StudyGroups,
    string? Description = null)
{
}