using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Columns;

public class GetColumnsQueryHandler : IRequestHandler<GetColumnsQuery, List<Column>?>
{
    private readonly IApplicationDbContext _context;
    
    public GetColumnsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Получаем все колонки указанной доски
    public async Task<List<Column>?> Handle(
        GetColumnsQuery request,
        CancellationToken cancellationToken)
    {
        // Проверяем, существует ли доска
        var boardExists = await _context.Boards
            .AnyAsync(
                b => b.Id == request.BoardId,
                cancellationToken);

        // Если доски нет - null
        if (!boardExists)
        {
            return null;
        }

        // Получаем колонки доски и сортируем их по позиции
        return await _context.Columns
            .Where(c => c.BoardId == request.BoardId)
            .OrderBy(c => c.Position)
            .ToListAsync(cancellationToken);
    }
}