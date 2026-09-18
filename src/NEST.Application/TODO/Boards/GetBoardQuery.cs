using MediatR;

namespace NEST.Application.TODO.Boards;

// Запрос на получение одной доски по ее Id
public record GetBoardQuery(Guid BoardId) : IRequest<BoardDto?>;