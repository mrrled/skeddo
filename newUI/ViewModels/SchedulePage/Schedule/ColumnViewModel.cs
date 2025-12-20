using Application.DtoModels;

public class ColumnViewModel
{
    public StudyGroupDto StudyGroup { get; set; }
    public StudySubgroupDto StudySubgroup { get; set; }
    public string DisplayName { get; set; }

    // Добавьте это свойство
    public bool IsSubgroupVisible => !string.IsNullOrEmpty(DisplayName) && StudySubgroup != null;

    public bool IsGroupColumn => StudySubgroup == null;
    public bool HasSubgroups => StudyGroup.StudySubgroups?.Count > 0;
    public int ColumnSpan => 1;
}