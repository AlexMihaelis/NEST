using MediatR;

namespace NEST.Application.TODO.Attachments;

// Описывает загрузку нового файла и связанные с ним данные
public class CreateAttachmentCommand : IRequest<Guid>
{
    public required Guid UploadedByUserId { get; set; }
    public required Stream FileStream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? CommentId { get; set; }
}