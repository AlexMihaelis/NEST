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
        // Ищем пользователя по Id
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id,
                cancellationToken);
        
        // Если пользователь не найден - сообщаем об этом
        if (user is null)
        {
            return false;
        }
        
        // Удаляем найденного пользователя из контекста EF Core
        _context.Users.Remove(user);
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        
        // Сообщаем, что пользователь успешно удален
        return true;
    }
}