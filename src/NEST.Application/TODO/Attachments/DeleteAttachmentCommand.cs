using MediatR;

namespace NEST.Application.TODO.Attachments;

// Команда на удаление Attachment и соответсвующего файла из хранилища
public record DeleteAttachmentCommand(Guid Id) :  IRequest<bool>;