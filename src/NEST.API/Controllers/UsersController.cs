using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.Users;
using NEST.Domain.Entities;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ISender позволяет отправлять команды и запросы через MediatR
    private readonly ISender _sender;
    
    public UsersController(ISender sender)
    {
        _sender = sender;
    }
    
    //POST: api/users
    // Создаем нового пользователя
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        // Передаем команду в MediatR
        // MediatR найдет подходящий Handler и запустит его 
        var userId = await _sender.Send(command, cancellationToken);
        
        // Возвращаем HTTP 201 Created и Id созданного пользователя
        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            userId);
    }
    
    // GET: api/users/{id}
    // Получаем пользователя по Id
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Создаем Query с Id пользователя, которого нужно найти
        var query = new GetUserByIdQuery
        {
            Id = id
        };
        
        // Передаем Query в MediatR
        // MediatR найдет GetUserByIdQueryHandler и запустит его
        var user = await _sender.Send(query, cancellationToken);
        
        // Если пользователь не найден - HTTP 404 Not Found
        if  (user is null)
        {
            return NotFound();
        }
        
        // Если пользователь найден - HTTP 200 OK
        return Ok(user);
    }
}