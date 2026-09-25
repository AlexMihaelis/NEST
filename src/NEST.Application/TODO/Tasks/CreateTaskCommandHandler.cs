using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid?>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var columnExists = await _context.Columns
            .AnyAsync(c => c.Id == request.ColumnId, cancellationToken);

        if (!columnExists)
        {
            return null;
        }
        
        if (request.TaskContextId.HasValue)
        {
            var taskContextExists = await _context.TaskContexts
                .AnyAsync(tc => tc.Id == request.TaskContextId.Value, cancellationToken);

            if (!taskContextExists)
            {
                return null;
            }
        }

        // Находим последнюю позицию задачи в колонке
        var maxPosition = await _context.Tasks
            .Where(t => t.ColumnId == request.ColumnId)
            .Select(t => (int?)t.Position)
            .MaxAsync(cancellationToken);

        // Если задач еще нет - первая получит позицию 0
        var position = (maxPosition ?? -1) + 1;

        var task = new Task
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            Deadline = request.Deadline,
            TaskContextId = request.TaskContextId,
            ColumnId = request.ColumnId,
            Position = position,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        
        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}