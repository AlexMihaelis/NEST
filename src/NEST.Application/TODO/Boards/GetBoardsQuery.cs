using MediatR;

namespace NEST.Application.TODO.Boards;

// Запрос на получение списка всех досок
public record GetBoardsQuery : IRequest<List<BoardDto>>;
