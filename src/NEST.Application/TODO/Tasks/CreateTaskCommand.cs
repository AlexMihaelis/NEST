using MediatR;
using NEST.Domain.Enums;

namespace NEST.Application.TODO.Tasks;

// Команда на создание новой задачи. В ответ вернет Guid новой задачи
public record CreateTaskCommand(
    string Name,
    string Description,
    TaskPriority Priority,
    DateTime? Deadline,
    Guid? TaskContextId,
    Guid ColumnId) :  IRequest<Guid?>;