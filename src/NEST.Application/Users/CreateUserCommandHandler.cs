using MediatR;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities;

namespace NEST.Application.Users;

// Handler выполняет действие, описанное в CreateUserCommand
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    
    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    // Обрабатываем команду создания пользователя
    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Создаем новую сущность пользователя на основе данных команд
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            BirthDate = request.BirthDate,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}