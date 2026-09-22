using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Columns;

// Handler выполняет действие, описанное в GetColumnQuery
public class GetColumnQueryHandler : IRequestHandler<GetColumnQuery, Column?>
{
    private readonly IApplicationDbContext _context;
    
    public GetColumnQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Получаем колонку по Id
    public async Task<Column?> Handle(
        GetColumnQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Columns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);
    }
}