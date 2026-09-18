using NEST.Domain.Entities.TODO;

namespace NEST.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}