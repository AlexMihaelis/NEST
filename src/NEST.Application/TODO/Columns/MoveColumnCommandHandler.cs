using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Columns;

// Handler выполняет действие, описанное в MoveColumnCommand
public class MoveColumnCommandHandler
    : IRequestHandler<MoveColumnCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public MoveColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        MoveColumnCommand request,
        CancellationToken cancellationToken)
    {
        var column = await _context.Columns
            .FirstOrDefaultAsync(
                c => c.Id == request.ColumnId,
                cancellationToken);

        if (column is null)
        {
            return false;
        }

        var oldPosition = column.Position;
        var boardId = column.BoardId;

        // Если колонка остается на том же месте,
        // ничего менять не нужно
        if (oldPosition == request.TargetPosition)
        {
            return true;
        }

        // Получаем количество остальных колонок на этой доске
        var columnsCount = await _context.Columns
            .CountAsync(
                c => c.BoardId == boardId &&
                     c.Id != column.Id,
                cancellationToken);

        // Ограничиваем позицию допустимым диапазоном
        var targetPosition = Math.Clamp(
            request.TargetPosition,
            0,
            columnsCount);

        // Если после ограничения позиция не изменилась,
        // ничего менять не нужно
        if (oldPosition == targetPosition)
        {
            return true;
        }

        // Перемещение колонки вниз
        if (targetPosition > oldPosition)
        {
            var columnsToShift = await _context.Columns
                .Where(c =>
                    c.BoardId == boardId &&
                    c.Position > oldPosition &&
                    c.Position <= targetPosition &&
                    c.Id != column.Id)
                .ToListAsync(cancellationToken);

            foreach (var columnToShift in columnsToShift)
            {
                columnToShift.Position--;
            }
        }
        // Перемещение колонки вверх
        else
        {
            var columnsToShift = await _context.Columns
                .Where(c =>
                    c.BoardId == boardId &&
                    c.Position >= targetPosition &&
                    c.Position < oldPosition &&
                    c.Id != column.Id)
                .ToListAsync(cancellationToken);

            foreach (var columnToShift in columnsToShift)
            {
                columnToShift.Position++;
            }
        }

        // Устанавливаем новую позицию перемещаемой колонки
        column.Position = targetPosition;
        column.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}