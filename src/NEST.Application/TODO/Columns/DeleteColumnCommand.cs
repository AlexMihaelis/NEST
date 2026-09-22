using MediatR;
using NEST.Domain.Enums;

namespace NEST.Application.TODO.Columns;

// Command (команда) - объект, который описывает действие, которое мы хотим выполнить

// Команда на удаление колонки
public record DeleteColumnCommand(Guid ColumnId, DeleteColumnMode Mode, Guid? TargetColumnId) : IRequest<bool>;