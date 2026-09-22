using MediatR;

namespace NEST.Application.TODO.Columns;

// Command (команда) - объект, который описывает действие, которое мы хотим выполнить

// Команда на создание новой колонки. В ответ Handler вернет Guid колонки
public class CreateColumnCommand : IRequest<Guid?>
{
    public required string Name { get; set; }
    public Guid BoardId { get; set; }
}