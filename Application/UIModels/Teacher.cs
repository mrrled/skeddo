namespace Application.UIModels;

public class Teacher
{
    public Teacher(string name, string surname, string middleName, string? specialty)
    {
        Name = name;
        Surname = surname;
        MiddleName = middleName;
        Specialty = specialty;
    }

    public string Name { get; set; }
    public string Surname { get; set; }
    
    public string MiddleName { get; set; }
    
    public string? Specialty { get; set; }
}