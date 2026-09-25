using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Boards;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    // ISender позволяет отправлять команды и запросы через MediatR
    private readonly ISender _sender;
    
    public BoardsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        // Создаем Query и отправляем его через MediatR
        var board = await _sender.Send(
            new GetBoardQuery(boardId),
            cancellationToken);
        
        if (board is null)
        {
            return NotFound();
        }
        
        return Ok(board);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        // Запрашиваем список досок через MediatR
        var boards = await _sender.Send(
            new GetBoardsQuery(),
            cancellationToken);
        
        return Ok(boards);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        // Передаем команду в MediatR, который найдет нужный Handler
        var boardId = await _sender.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(Get), new { boardId }, new { id = boardId });
    }

    [HttpPut("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid boardId,
        UpdateBoardCommand command,
        CancellationToken cancellationToken)
    {
        // Создаем команду для обновления доски. Id доски берем из URL, остальное - из тела запроса
        var updateBoard = new UpdateBoardCommand(
            boardId,
            command.Name,
            command.Description);
        
        // Передаем команду в MediatR
        var updated = await _sender.Send(
            updateBoard,
            cancellationToken);
        
        if (!updated)
        {
            return NotFound();
        }
        
        return NoContent();
    }
    
    [HttpDelete("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        // Создаем команду на удаление
        var command = new DeleteBoardCommand(boardId);
        
        // Передаем команду в MediatR
        var deleted = await _sender.Send(
            command,
            cancellationToken);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}