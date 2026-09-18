namespace NEST.Domain.Entities.TODO;

public class Column
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; }
    
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Position { get; set; }
}