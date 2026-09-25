using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

// Handler выполняет действие, описанное в MoveTaskCommand
public class MoveTaskCommandHandler
    : IRequestHandler<MoveTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public MoveTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        MoveTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(
                t => t.Id == request.TaskId,
                cancellationToken);
        
        if (task is null)
        {
            return false;
        }

        // Проверяем, существует ли целевая колонка
        var targetColumnExists = await _context.Columns
            .AnyAsync(
                c => c.Id == request.TargetColumnId,
                cancellationToken);
        
        if (!targetColumnExists)
        {
            return false;
        }

        var oldColumnId = task.ColumnId;
        var oldPosition = task.Position;

        // Перемещение внутри той же колонки
        // else: Перемещение в другую колонку
        if (oldColumnId == request.TargetColumnId)
        {
            // Если задача остаётся на том же месте - ничего менять не нужно
            if (oldPosition == request.TargetPosition)
            {
                return true;
            }

            // Перемещаем задачу вниз
            if (request.TargetPosition > oldPosition)
            {
                var tasksToShift = await _context.Tasks
                    .Where(t =>
                        t.ColumnId == oldColumnId &&
                        t.Position > oldPosition &&
                        t.Position <= request.TargetPosition &&
                        t.Id != task.Id)
                    .ToListAsync(cancellationToken);

                foreach (var taskToShift in tasksToShift)
                {
                    taskToShift.Position--;
                }
            }
            // Перемещаем задачу вверх
            else
            {
                var tasksToShift = await _context.Tasks
                    .Where(t =>
                        t.ColumnId == oldColumnId &&
                        t.Position >= request.TargetPosition &&
                        t.Position < oldPosition &&
                        t.Id != task.Id)
                    .ToListAsync(cancellationToken);

                foreach (var taskToShift in tasksToShift)
                {
                    taskToShift.Position++;
                }
            }

            task.Position = request.TargetPosition;
        }
        else
        {
            // Освобождаем позицию в старой колонке
            var tasksInOldColumn = await _context.Tasks
                .Where(t =>
                    t.ColumnId == oldColumnId &&
                    t.Position > oldPosition &&
                    t.Id != task.Id)
                .ToListAsync(cancellationToken);

            foreach (var taskToShift in tasksInOldColumn)
            {
                taskToShift.Position--;
            }

            // Получаем задачи целевой колонки
            var tasksInTargetColumn = await _context.Tasks
                .Where(t => t.ColumnId == request.TargetColumnId)
                .ToListAsync(cancellationToken);

            // Определяем допустимую позицию. Если позиция больше количества задач, ставим задачу в конец.
            var targetPosition = Math.Min(
                request.TargetPosition,
                tasksInTargetColumn.Count);

            // Освобождаем место в целевой колонке
            foreach (var taskToShift in tasksInTargetColumn
                         .Where(t => t.Position >= targetPosition))
            {
                taskToShift.Position++;
            }

            // Меняем колонку и позицию задачи
            task.ColumnId = request.TargetColumnId;
            task.Position = targetPosition;
        }
        
        task.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}