using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Attachments;

// Удаляет файл из хранилища и соответствующую запись из бд
public class DeleteAttachmentCommandHandler
    : IRequestHandler<DeleteAttachmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public DeleteAttachmentCommandHandler(
        IApplicationDbContext context,
        IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<bool> Handle(
        DeleteAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        // Находим Attachment, чтобы получить StorageKey для удаления файла из MinIO
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(
                a => a.Id == request.Id,
                cancellationToken);

        // Если Attachment не найден - сообщаем
        if (attachment is null)
        {
            return false;
        }

        // Сначала удаляем сам файл из MinIO
        await _fileStorage.DeleteAsync(
            attachment.StorageKey,
            cancellationToken);

        // После успешного удаления файла удаляем его метаданные из бд
        _context.Attachments.Remove(attachment);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}