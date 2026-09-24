using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities;

namespace NEST.Application.Users;

// Handler выполняет запрос GetUsersQuery
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<User>>
{
    private readonly IApplicationDbContext _context;
    
    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем запрос получения всех пользователей
    public async Task<List<User>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .ToListAsync(cancellationToken);
        
        return users;
    }
}