using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.Users;

// Handler выполняет действие, описанное в DeleteUserCommand
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public DeleteUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем команду удаления пользователя
    public async Task<bool> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id,
                cancellationToken);
        
        if (user is null)
        {
            return false;
        }
        
        _context.Users.Remove(user);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}