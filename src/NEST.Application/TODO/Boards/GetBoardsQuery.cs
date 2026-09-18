using MediatR;

namespace NEST.Application.TODO.Boards;

public record GetBoardsQuery : IRequest<List<BoardDto>>;
