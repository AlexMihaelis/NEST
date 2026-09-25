using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

// Handler выполняет действие, описанное в GetTaskQuery
public class GetColumnQueryHandler : IRequestHandler<GetTaskQuery, Task?>
{
    private readonly IApplicationDbContext _context;

    public GetColumnQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Task?> Handle(
        GetTaskQuery request,
        CancellationToken cancellationToken)
    {
        // Ищем задачу по Id. Если не найдена - null
        return await  _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, 
                cancellationToken);
    }
}