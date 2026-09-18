using MediatR;

namespace NEST.Application.TODO.Boards;

// Команда на удаление существующей доски
public record DeleteBoardCommand(Guid BoardId) : IRequest<bool>;