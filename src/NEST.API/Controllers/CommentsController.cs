using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.API.Controllers.Requests.Comments;
using NEST.Application.TODO.Comments;

namespace NEST.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ISender _sender;
    
    public CommentsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var commentId = await _sender.Send(
            new CreateCommentCommand(
                request.Content,
                request.TaskId,
                request.UserId),
            cancellationToken);
        
        if (commentId is null)
        {
            return NotFound();
        }
        
        return CreatedAtAction(nameof(Get), new { commentId }, new { commentId });
    }
    
    [HttpGet("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var comment = await _sender.Send(
            new GetCommentQuery(commentId),
            cancellationToken);
        
        if (comment is null)
        {
            return NotFound();
        }

        return Ok(comment);
    }
    
    [HttpGet("/api/tasks/{taskId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTask(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var comments = await _sender.Send(
            new GetCommentsQuery(taskId), cancellationToken);
        
        if (comments is null)
        {
            return NotFound();
        }
        
        return Ok(comments);
    }
    
    [HttpPut("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid commentId,
        UpdateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(
            new UpdateCommentCommand(
                commentId,
                request.Content),
            cancellationToken);
        
        if (!updated)
        {
            return NotFound();
        }
        
        return NoContent();
    }
    
    [HttpDelete("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeleteCommentCommand(commentId),
            cancellationToken);
        
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}