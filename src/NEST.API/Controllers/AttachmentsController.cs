using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Attachments;

namespace NEST.API.Controllers;

// HTTP-специфичные типы, например IFormFile, остаются только в API-слое
[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttachmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Загружает файл и передает его в Application-слой для обработки
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        IFormFile file,
        [FromForm] Guid uploadedByUserId,
        [FromForm] Guid? taskId,
        [FromForm] Guid? commentId,
        CancellationToken cancellationToken)
    {
        // IFormFile дает доступ к потоку с содержимым загруженного файла
        // Поток нужен Application-слою для передачи файла в хранилище
        await using var stream = file.OpenReadStream();

        // Преобразуем HTTP-модель IFormFile в команду Application-слоя
        var command = new CreateAttachmentCommand
        {
            UploadedByUserId = uploadedByUserId,
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            TaskId = taskId,
            CommentId = commentId
        };

        // Передаем команду MediatR, который найдет соответствующий Handler
        var attachmentId = await _mediator.Send(command, cancellationToken);

        // Возвращаем Id созданного Attachment и статус 201 Created 
        return CreatedAtAction(
            nameof(Create),
            new { id = attachmentId },
            new { id = attachmentId });
    }
}