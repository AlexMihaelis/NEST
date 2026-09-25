using MediatR;
using NEST.Domain.Enums;

namespace NEST.Application.TODO.Tasks;

public record UpdateTaskCommand(
    Guid TaskId,
    string Name,
    string Description,
    TaskPriority Priority,
    DateTime? Deadline,
    bool IsCompleted,
    Guid? TaskContextId) : IRequest<bool>;