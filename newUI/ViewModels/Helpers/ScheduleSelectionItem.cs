namespace newUI.ViewModels.Helpers;

public class ScheduleSelectionItem
{
    public ScheduleSelectionItem(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    
    public override bool Equals(object obj)
    {
        return obj is ScheduleSelectionItem other && Id == other.Id;
    }
    
    public override int GetHashCode() => Id.GetHashCode();
}