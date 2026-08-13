namespace NEST.Domain.Entities.TODO;

public class Attachment
{
    public Guid Id { get; set; }
    
    public Guid? TaskId { get; set; }
    public Task? Task { get; set; }

    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
    
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string StorageKey { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Guid UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; }
}