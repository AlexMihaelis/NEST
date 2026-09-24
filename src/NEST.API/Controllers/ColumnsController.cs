using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Columns;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColumnsController : ControllerBase
{
    // ISender позволяет отправлять команды и запросы через MediatR
    private readonly ISender _sender;
    
    public ColumnsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var columnId = await _sender.Send(command, cancellationToken);
        
        if (columnId is null)
        {
            return NotFound();
        }
        
        return Created($"api/columns/{columnId}", new {id = columnId});
    }
    
    [HttpGet("{columnId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        Guid columnId, 
        CancellationToken cancellationToken)
    {
        var column = await _sender.Send(
            new GetColumnQuery(columnId),
            cancellationToken);
        
        if (column is null)
        {
            return NotFound();
        }
        
        return Ok(column);
    }
    
    [HttpGet("/api/boards/{boardId:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByBoard(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var columns = await _sender.Send(
            new GetColumnsQuery(boardId),
            cancellationToken);
        
        if (columns is null)
        {
            return NotFound();
        }

        return Ok(columns);
    }
    
    [HttpPut("{columnId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid columnId,
        UpdateColumnRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(
            new UpdateColumnCommand(
                columnId,
                request.Name,
                request.Position),
            cancellationToken);
        
        if (!updated)
        {
            return NotFound();
        }   
        
        return NoContent();
    }
    
    [HttpDelete("{columnId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid columnId,
        DeleteColumnRequest request,
        CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeleteColumnCommand(
                columnId,
                request.Mode,
                request.TargetColumnId),
            cancellationToken);
        
        if  (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}