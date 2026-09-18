using MediatR;

namespace NEST.Application.TODO.Boards;

// Команда обновления существующей доски
public record UpdateBoardCommand(
    Guid BoardId,
    string Name,
    string? Description
    ) :  IRequest<bool>;