using MediatR;
using NEST.Application.Common.Interfaces;
using Board = NEST.Domain.Entities.TODO.Board;

namespace NEST.Application.TODO.Boards;

public class CreateBoardCommandHandler
    : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        // Создаем новую сущность Board на основе данных из команды
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Добавляем новую сущность в DbContext
        _context.Boards.Add(board);

        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);

        // Возвращаем Id созданной доски
        return board.Id;
    }
}