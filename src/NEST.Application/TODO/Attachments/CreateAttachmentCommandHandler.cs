using MediatR;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Attachments;

// Загружает файл в хранилище и сохраняет информацию о нем в бд
public class CreateAttachmentCommandHandler : IRequestHandler<CreateAttachmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public CreateAttachmentCommandHandler(
        IApplicationDbContext context,
        IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Guid> Handle(
        CreateAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var attachmentId = Guid.NewGuid();

        var safeFileName = Path.GetFileName(request.FileName);

        var storageKey = $"attachments/{attachmentId}/{safeFileName}";

        var attachment = new Attachment
        {
            Id = attachmentId,
            TaskId = request.TaskId,
            CommentId = request.CommentId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            Size = request.FileStream.Length,
            StorageKey = storageKey,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UploadedByUserId = request.UploadedByUserId
        };

        await _fileStorage.UploadAsync(
            request.FileStream,
            storageKey,
            request.ContentType,
            cancellationToken);

        try
        {
            _context.Attachments.Add(attachment);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Если запись в бд не сохранилась, удаляем уже загруженный файл из MinIO
            await _fileStorage.DeleteAsync(storageKey, cancellationToken);

            throw;
        }

        return attachmentId;
    }
}