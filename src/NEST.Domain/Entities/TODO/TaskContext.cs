namespace NEST.Domain.Entities.TODO;

public class TaskContext
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}