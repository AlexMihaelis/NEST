using MediatR;

namespace NEST.Application.TODO.Tasks;

// Команда на удаление существующей задачи
public record DeleteTaskCommand(Guid TaskId) : IRequest<bool>;