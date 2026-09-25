using MediatR;
using Microsoft.AspNetCore.Mvc;
using NEST.API.Controllers.Requests;
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
        
        if (taskId is null)
        {
            return NotFound();
        }
    
        return CreatedAtAction(
            nameof(Get), new { taskId }, new { taskId });
    }
    
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
        
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }
    
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
        
        if (tasks is null)
        {
            return NotFound();
        }
        
        return Ok(tasks);
    }
    
    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid taskId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sender.Send(
            new UpdateTaskCommand(
                taskId,
                request.Name,
                request.Description,
                request.Priority,
                request.Deadline,
                request.IsCompleted,
                request.TaskContextId),
            cancellationToken);
        
        if (!updated)
        {
            return NotFound();
        }
    
        return NoContent();
    }
    
    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(
            new DeleteTaskCommand(taskId),
            cancellationToken);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
    
    // Перемещение задачи в другую позицию/колонку
    [HttpPut("{taskId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Move(
        Guid taskId,
        MoveTaskRequest request,
        CancellationToken cancellationToken)
    {
        var moved = await _sender.Send(
            new MoveTaskCommand(
                taskId,
                request.TargetColumnId,
                request.TargetPosition),
            cancellationToken);
        
        if (!moved)
        {
            return NotFound();
        }

        return NoContent();
    }
}