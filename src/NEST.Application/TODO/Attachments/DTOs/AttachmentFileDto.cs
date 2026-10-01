namespace NEST.Application.TODO.Attachments.DTOs;

public class AttachmentFileDto
{
    public required Stream FileStream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
}