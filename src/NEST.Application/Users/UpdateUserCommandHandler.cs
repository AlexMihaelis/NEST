using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.Users;

// Handler выполняет действие, описанное в UpdateUserCommand
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем команду обновления пользователя
    public async Task<bool> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id,
                cancellationToken);
        
        if (user is null)
        {
            return false;
        }
        
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.BirthDate = request.BirthDate;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}