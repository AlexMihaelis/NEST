using MediatR;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

// Query (запрос) для получения одной задачи
public record GetTaskQuery(Guid TaskId) : IRequest<Task?>;