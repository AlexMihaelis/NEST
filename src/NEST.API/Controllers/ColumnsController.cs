using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Columns;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColumnsController : ControllerBase
{
    private readonly ISender _sender;
    
    public ColumnsController(ISender sender)
    {
        _sender = sender;
    }

    // Создание новой колонки
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var columnId = await _sender.Send(command, cancellationToken);
        
        // Если доска не найена - null
        if (columnId is null)
        {
            return NotFound();
        }
        
        return Created($"api/columns/{columnId}", new {id = columnId});
    }
    
    // Получение колонки по Id
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
        
        // Если колонка не найдена - 404
        if (column is null)
        {
            return NotFound();
        }
        
        return Ok(column);
    }
    
    // Получение всех колонок у доски
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

        // Если доска не найдена - 404
        if (columns is null)
        {
            return NotFound();
        }

        return Ok(columns);
    }
    
    // Обновление колонки
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
        
        // Если колонка не найдена - 404
        if (!updated)
        {
            return NotFound();
        }   
        
        return NoContent();
    }
    
    // Удаление колонки
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
        
        // Если колонка или целевая колонка не найдены - 404
        if  (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}