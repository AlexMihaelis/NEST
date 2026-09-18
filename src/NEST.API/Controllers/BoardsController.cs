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
    public async Task<IActionResult> Get(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        // Создаем Query и отправляем его через MediatR
        var board = await _sender.Send(
            new GetBoardQuery(boardId),
            cancellationToken);

        // Если доска с таким ID не найдена - возвращаем 404
        if (board is null)
        {
            return NotFound();
        }
        
        // Если доска найдена — возвращаем её данные с кодом 200.
        return Ok(board);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        // Запрашиваем список досок через MediatR
        var boards = await _sender.Send(
            new GetBoardsQuery(),
            cancellationToken);
        
        return Ok(boards);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        // Передаем команду в MediatR, который найдет нужный Handler
        var boardId = await _sender.Send(command, cancellationToken);
        
        // Возвращаем 201 Created и Id созданной доски
        return Created(
            $"/api/boards/{boardId}",
            new { id = boardId });
    }

    [HttpPut("{boardId:guid}")]
    public async Task<IActionResult> Update(
        Guid boardId,
        UpdateBoardCommand command,
        CancellationToken cancellationToken)
    {
        //Создаем команду для ообновления доски
        // Id доски берем из URL, остальное - из тела запроса
        var updateBoard = new UpdateBoardCommand(
            boardId,
            command.Name,
            command.Description);
        
        // Передаем команду в MediatR
        var updated = await _sender.Send(
            updateBoard,
            cancellationToken);
        
        // Если доска не найдена - 404
        if (!updated)
        {
            return NotFound();
        }
        
        // Обновление прошло успешно (возвращать данные не нужно)
        return NoContent();
    }
    
    [HttpDelete("{boardId:guid}")]
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
        
        // Если доска не найдена - 404
        if (!deleted)
        {
            return NotFound();
        }
        
        // Доска успешно удалена
        return NoContent();
    }
}