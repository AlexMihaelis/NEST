using MediatR;

namespace NEST.Application.TODO.Tasks;

// Команда на перемещение задачи в другую позицию и/или колонку
public record MoveTaskCommand(Guid TaskId, Guid TargetColumnId, int TargetPosition) : IRequest<bool>;