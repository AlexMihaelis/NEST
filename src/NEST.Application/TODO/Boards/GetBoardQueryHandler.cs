using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

// Handler (обработчик) для получения одной доски по ее Id
public class GetBoardQueryHandler
    : IRequestHandler<GetBoardQuery, BoardDto?>
{
    // Интерфейс для работы с бд через Application-слой
    private readonly IApplicationDbContext _context;

    public GetBoardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BoardDto?> Handle(
        GetBoardQuery query,
        CancellationToken cancellationToken)
    {
        // Говорим EF Core не отслеживать сущность, потому что мы только читаем данные и не собираемся их изменять. Т.е. только для чтения
        // Оставляем только ту доску, Id которой совпадает с переданным в Query
        // Преобразуем найденную сущность Board в BoardDto
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

            // Получаем первую найденную запись или null, если доска не найдена
            .FirstOrDefaultAsync(cancellationToken);
    }
}