namespace Domain.Models;

public class Teacher(
    int id,
    FullName fullName,
    List<SchoolSubject> schoolSubjects,
    List<StudyGroup> studyGroups) : Entity<int>(id)
{
    public FullName FullName { get; set; } = fullName;
    public List<SchoolSubject> SchoolSubjects { get; set; } = schoolSubjects;
    public List<StudyGroup> StudyGroups { get; set; } = studyGroups;
}