using MediatR;

namespace NEST.Application.TODO.Columns;

// Команда на перемещение колонки в другую позицию
public record MoveColumnCommand(
    Guid ColumnId,
    int TargetPosition) : IRequest<bool>;