using MediatR;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Columns;

// Запрос на получение всех колонок определенной доски
public record GetColumnsQuery(Guid BoardId) : IRequest<List<Column>?>;