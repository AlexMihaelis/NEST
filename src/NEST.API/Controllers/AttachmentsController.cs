using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Attachments;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly ISender _sender;

    public AttachmentsController(ISender sender)
    {
        _sender = sender;
    }
    
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
        
        return CreatedAtAction(
            nameof(Create),
            new { id = attachmentId },
            new { id = attachmentId });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var attachment = await _sender.Send(
            new GetAttachmentQuery { Id = id },
            cancellationToken);
        
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
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeleteAttachmentCommand(id),
            cancellationToken);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}