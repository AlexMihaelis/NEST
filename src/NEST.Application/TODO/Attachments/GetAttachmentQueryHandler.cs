using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Application.TODO.Attachments.DTOs;

namespace NEST.Application.TODO.Attachments;

// Находим Attachment в БД и получаем соответствующий файл из хранилища 
public class GetAttachmentQueryHandler : IRequestHandler<GetAttachmentQuery, AttachmentFileDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public GetAttachmentQueryHandler(IApplicationDbContext context,  IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<AttachmentFileDto?> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        // Attachment нужен для получения StorageKey и информации, необходимой для формирования ответа
        var attachment = await _context.Attachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        // Если записи о файле нет - null
        if (attachment is null)
        {
            return null;
        }
        
        // По StorageKey получаем содержимое файла из MinIO
        var fileStream = await _fileStorage.DownloadAsync(
            attachment.StorageKey,
            cancellationToken);

        return new AttachmentFileDto
        {
            FileStream = fileStream,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType
        };
    }
}