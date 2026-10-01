using MediatR;
using Microsoft.EntityFrameworkCore;
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
        // Если указан TaskId, проверяем, что такой Task существует
        if (request.TaskId.HasValue)
        {
            var taskExists = await _context.Tasks
                .AnyAsync(
                    t => t.Id == request.TaskId.Value,
                    cancellationToken);

            if (!taskExists)
            {
                throw new KeyNotFoundException(
                    $"Task with id '{request.TaskId}' was not found.");
            }
        }

        // Если указан CommentId, получаем комментарий вместе с TaskId, чтобы проверить существование комментария и его связь с Task
        if (request.CommentId.HasValue)
        {
            var commentTaskId = await _context.Comments
                .Where(c => c.Id == request.CommentId.Value)
                .Select(c => (Guid?)c.TaskId)
                .FirstOrDefaultAsync(cancellationToken);

            if (commentTaskId is null)
            {
                throw new KeyNotFoundException(
                    $"Comment with id '{request.CommentId}' was not found.");
            }

            // Если указаны и TaskId, и CommentId, комментарий должен принадлежать этому Task
            if (request.TaskId.HasValue &&
                commentTaskId.Value != request.TaskId.Value)
            {
                throw new InvalidOperationException(
                    "The specified comment does not belong to the specified task.");
            }
        }

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
            await _fileStorage.DeleteAsync(
                storageKey,
                cancellationToken);

            throw;
        }

        return attachmentId;
    }
}