using MediatR;

namespace NEST.Application.TODO.Columns;

// Команда на обновление колонки. В ответ ничего не возвращает
public record UpdateColumnCommand(Guid ColumnId, string Name, int Position) : IRequest<bool>;