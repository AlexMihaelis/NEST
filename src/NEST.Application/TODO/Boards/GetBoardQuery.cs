using MediatR;

namespace NEST.Application.TODO.Boards;

public record GetBoardQuery(Guid BoardId) : IRequest<BoardDto?>;