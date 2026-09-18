using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

public class GetBoardsQueryHandler : IRequestHandler<GetBoardsQuery, List<BoardDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBoardsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BoardDto>> Handle(
        GetBoardsQuery query,
        CancellationToken cancellationToken)
    {
        // Получаем все доски и выбираем только нужные поля
        // AsNoTracking() - говорим EF, что только для чтения
        return await _context.Boards
            .AsNoTracking()
            .Select(board => new BoardDto(
                board.Id,
                board.Name,
                board.Description,
                board.UserId,
                board.CreatedAt,
                board.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}