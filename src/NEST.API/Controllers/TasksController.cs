using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.Application.TODO.Tasks;

namespace NEST.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ISender _sender;
    
    public TasksController(ISender sender)
    {
        _sender = sender;
    }
    
    // Создание новой задачи
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var taskId = await _sender.Send(
            new CreateTaskCommand(
                request.Name,
                request.Description,
                request.Priority,
                request.Deadline,
                request.TaskContextId,
                request.ColumnId),
            cancellationToken);
        
        // Если колонка или котекст не найдены - 404
        if (taskId is null)
        {
            return NotFound();
        }
        
        //return Created(nameof(Get), new {taskId}, null);
        return Created($"api/tasks/{taskId}", new {taskId});
    }
    
    // Получение задачи по Id
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var task = await _sender.Send(
            new GetTaskQuery(taskId),
            cancellationToken);
        
        // Если задача не найдена - 404
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }
    
    // Получение всех задач колонки
    [HttpGet("/api/columns/{columnId:guid}/tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByColumn(
        Guid columnId,
        CancellationToken cancellationToken)
    {
        var tasks = await _sender.Send(
            new GetTasksQuery(columnId),
            cancellationToken);
        
        // Если колонка не найдена - 404
        if (tasks is null)
        {
            return NotFound();
        }
        
        return Ok(tasks);
    }
}