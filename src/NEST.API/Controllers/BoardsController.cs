using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Boards;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly ISender _sender;
    
    public BoardsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        var boardId = await _sender.Send(command, cancellationToken);
        
        return Created(
            $"/api/boards/{boardId}",
            new { id = boardId });
    }
}