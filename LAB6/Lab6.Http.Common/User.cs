namespace Lab6.Http.Common;

public class TaskItem
{
    public TaskItem()
    {
    }

    public TaskItem(int id, string name, string duration)
    {
        Id = id;
        Name = name;
        Duration = duration;
        
    }

    public int Id { get; set; }

    public string Duration { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

}
