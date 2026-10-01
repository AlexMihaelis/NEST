using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Attachments;

namespace NEST.API.Controllers;

// HTTP-специфичные типы, например IFormFile, остаются только в API-слое
[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly ISender _sender;

    public AttachmentsController(ISender sender)
    {
        _sender = sender;
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
        var attachmentId = await _sender.Send(command, cancellationToken);

        // Возвращаем Id созданного Attachment и статус 201 Created 
        return CreatedAtAction(
            nameof(Create),
            new { id = attachmentId },
            new { id = attachmentId });
    }
    
    // Возвращает файл Attachment по его идентификатору
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Получаем файл через Application-слой
        var attachment = await _sender.Send(
            new GetAttachmentQuery { Id = id },
            cancellationToken);

        // Если Attachment с таким ID не найден - 404
        if (attachment is null)
        {
            return NotFound();
        }

        // File() формирует HTTP-ответ с содержимым файла, его MIME-типом и исходным именем
        return File(
            attachment.FileStream,
            attachment.ContentType,
            attachment.FileName);
    }
    
    // Удаляет Attachment и соответствющий файл из хранилища
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        // Передаем команду на удаление в Application-слой
        var deleted = await _sender.Send(
            new DeleteAttachmentCommand(id),
            cancellationToken);
        
        // Если Attachment не найден - 404
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}