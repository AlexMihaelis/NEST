using MediatR;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Columns;

// Query (запрос) - объект, который описывает данные, которые мы хотим получить

//Запрос на получение колонки по ее Id. Handler вернет колонку или null (если ее не существует)
public record GetColumnQuery(Guid ColumnId) : IRequest<Column?>;