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
        // Ищем доску по Id
        var board = await _context.Boards
            .FirstOrDefaultAsync(
                b => b.Id == command.BoardId,
                cancellationToken);
        
        // Если доска не найдена - false
        if (board is null)
        {
            return false;
        }
        
        // Помечаем доску для удаление
        _context.Boards.Remove(board);
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        
        // Сообщаем, что удаление прошло успешко
        return true;
    }
}