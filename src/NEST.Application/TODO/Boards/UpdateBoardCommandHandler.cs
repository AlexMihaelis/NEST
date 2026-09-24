using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

// Handler для обновления существующей доски
public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        UpdateBoardCommand command,
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
        
        board.Name = command.Name;
        board.Description = command.Description;
        board.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}