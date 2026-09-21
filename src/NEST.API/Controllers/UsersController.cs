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
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    
    // GET: api/users
    // Получаем всех пользователей
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll(CancellationToken cancellationToken)
    {
        // Создаем Query для получения всех пользователей
        var query = new GetUsersQuery();
        
        // Передаем Query в MediatR
        // MediatR найдет GetUsersQueryHandler и запустит его
        var users = await _sender.Send(query, cancellationToken);
        
        // Возвращаем HTTP 200 OK со списком пользователей
        return Ok(users);
    }
    
    // PUT: api/users/{id}
    // Обновляем существующего пользователя
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        // Создаем команду для Application-слоя
        // Id берем из URL, а остальные данные - из тела запроса
        var command = new UpdateUserCommand
        {
            Id = id,
            UserName = request.UserName,
            Email = request.Email,
            BirthDate = request.BirthDate
        };

        // Передаем команду в MediatR
        // MediatR найдет UpdateUserCommandHandler и запустит его
        var updated = await _sender.Send(command, cancellationToken);

        // Если пользователь не найден - HTTP 404 Not Found
        if (!updated)
        {
            return NotFound();
        }

        // Если пользователь обновлен - HTTP 204 No Content
        return NoContent();
    }
    
    // DELETE: api/users/{id}
    // Удаляем существующего пользователя
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Создаем команду на удаление пользователя. Id берем из URL
        var command = new DeleteUserCommand
        {
            Id = id
        };
        
        // Передаем команду в MediatR
        // MediatR найдет DeleteUserCommandHandler и запустит его
        var deleted = await _sender.Send(command, cancellationToken);
        
        // Если пользователь не найден - HTTP 404 Not Found
        if (!deleted)
        {
            return NotFound();
        }
        
        // Если пользователь удален - HTTP 204 No Content
        return NoContent();
    }
}