using MediatR;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

// Query (запрос) для полуечния всех задач колонки
public record GetTasksQuery(Guid ColumnId) : IRequest<List<Task>?>;