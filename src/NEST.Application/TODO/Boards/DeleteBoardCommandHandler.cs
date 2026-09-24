using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public DeleteBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteBoardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(
                b => b.Id == command.BoardId,
                cancellationToken);
        
        if (board is null)
        {
            return false;
        }
        
        _context.Boards.Remove(board);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}