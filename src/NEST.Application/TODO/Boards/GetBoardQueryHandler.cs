using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

public class GetBoardQueryHandler : IRequestHandler<GetBoardQuery, BoardDto?>
{
    private readonly IApplicationDbContext _context;

    public GetBoardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BoardDto?> Handle(
        GetBoardQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.Boards
            .AsNoTracking()
            .Where(board => board.Id == query.BoardId)
            .Select(board => new BoardDto(
                board.Id,
                board.Name,
                board.Description,
                board.UserId,
                board.CreatedAt,
                board.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}