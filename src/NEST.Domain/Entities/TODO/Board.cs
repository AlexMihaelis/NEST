namespace NEST.Domain.Entities.TODO;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Column> Columns { get; set; } = new List<Column>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}