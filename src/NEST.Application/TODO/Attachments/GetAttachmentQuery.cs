using MediatR;
using NEST.Application.TODO.Attachments.DTOs;

namespace NEST.Application.TODO.Attachments;

// Запрашиваем данные Attachment по его Id
public class GetAttachmentQuery : IRequest<AttachmentFileDto?>
{
    public Guid Id { get; set; }
}