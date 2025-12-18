using Application.DtoModels;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class ColumnViewModel
{
    public StudyGroupDto StudyGroup { get; set; }
    public StudySubgroupDto StudySubgroup { get; set; } // null = вся группа
    public string DisplayName { get; set; }
    
    public bool HasSubgroups => StudyGroup.StudySubgroups?.Count > 0;
    public int ColumnSpan => StudyGroup.StudySubgroups?.Count > 0 ? StudyGroup.StudySubgroups.Count : 1;
}