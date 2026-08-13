using NEST.Domain.Enums;

namespace NEST.Domain.Entities.TODO;

public class Task
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; }
    
    public Guid? TaskContextId { get; set; }
    public TaskContext? TaskContext { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    
    public Guid ColumnId { get; set; }
    public Column Column { get; set; }
    
    public int Position { get; set; }
}