namespace NEST.Domain.Entities.TODO;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; }

    public Guid TaskId { get; set; }
    public Task? Task { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}