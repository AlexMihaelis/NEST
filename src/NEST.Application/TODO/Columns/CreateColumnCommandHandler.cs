using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Columns;

// Handler выполняет действие, описанное в CreateColumnCommand
public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Guid?>
{
    private readonly IApplicationDbContext _context;
    
    public CreateColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем команду создания колонки
    public async Task<Guid?> Handle(
        CreateColumnCommand request,
        CancellationToken cancellationToken)
    {
        // Проверяем, существует ли доска, к которой хотим добавить колонку
        var boardExists = await _context.Boards
            .AnyAsync(
                b => b.Id == request.BoardId,
                cancellationToken);
        
        if (!boardExists)
        {
            return null;
        }
        
        // Находим максимальную позицию среди колонок на доске. Если колонки еще нет - null
        var maxPosition = await _context.Columns
            .Where(c => c.BoardId == request.BoardId)
            .Select(c => (int?)c.Position)
            .MaxAsync(cancellationToken);
        
        // Если колонок нет - начинаем с 0-ой позиции. Иначе ставим новую колонку после последней
        var position = (maxPosition ?? -1) + 1;
        
        // Созадем новую колонку
        var column = new Column
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BoardId = request.BoardId,
            Position = position,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Columns.Add(column);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return column.Id;
    }
}