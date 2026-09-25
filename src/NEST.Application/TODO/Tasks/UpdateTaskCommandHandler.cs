using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Tasks;

// Handler выполняет действие, описанное 
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);
        
        if (task is null)
        {
            return false;
        }
        
        // Если указан контекст, проверяем, существует ли он
        if (request.TaskContextId.HasValue)
        {
            var taskContextExists = await _context.TaskContexts
                .AnyAsync(tc => tc.Id == request.TaskContextId.Value, cancellationToken);
            
            if (!taskContextExists)
            {
                return false;
            }
        }
        
        task.Name = request.Name;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Deadline = request.Deadline;
        task.IsCompleted = request.IsCompleted;
        task.TaskContextId = request.TaskContextId;
        task.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}