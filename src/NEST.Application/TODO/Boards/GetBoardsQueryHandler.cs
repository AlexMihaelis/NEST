using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Boards;

// Handler (обработчик) для получения одной доски по ее Id
public class GetBoardsQueryHandler : IRequestHandler<GetBoardsQuery, List<BoardDto>>
{
    // Интерфейс для работы с бд через Application-слой
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
        // AsNoTracking() - говорим EF Core не отслеживать сущность, потому что мы только читаем данные и не собираемся их изменять. Т.е. только для чтения
        // Преобразуем найденную сущность Board в BoardDto
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