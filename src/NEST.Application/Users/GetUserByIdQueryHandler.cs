using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities;

namespace NEST.Application.Users;

// Handler выполняет запрос GetUserByIdQuery
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
{
    private readonly IApplicationDbContext _context;
    
    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем запрос получения пользователя по Id
    public async Task<User?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Ищем пользователя по Id
        // Если прользователь не найден, FirstOrDefaultAsync вернет null
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.Id,
                cancellationToken);
        
        // Возвращаем найденного пользователя или null
        return user;
    }
}