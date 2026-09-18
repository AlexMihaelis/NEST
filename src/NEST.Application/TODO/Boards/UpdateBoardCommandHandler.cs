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
        // Ищем доску по Id
        var board = await _context.Boards
            .FirstOrDefaultAsync(
                b => b.Id == command.BoardId,
                cancellationToken);
        
        // Если доска не найдена, то сообщаем об этом
        if (board is null)
        {
            return false;
        }
        
        // Обновляем данные существующей доски
        board.Name = command.Name;
        board.Description = command.Description;
        board.UpdatedAt = DateTime.UtcNow;
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        
        // Сообщаем, что обновление прошло успешно
        return true;
    }
}