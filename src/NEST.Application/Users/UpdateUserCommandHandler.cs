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
        // Ищем пользователя по Id
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id,
                cancellationToken);
        
        // Если пользователь не найден - сообщаем об этом
        if (user is null)
        {
            return false;
        }
        
        // Обновляем данные найденного пользователя
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.BirthDate = request.BirthDate;
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);

        // Сообщаем, что пользователь успешно обновлен
        return true;
    }
}