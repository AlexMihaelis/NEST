using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в CreateCommentCommand
public class CreateCommentCommandHandler
    : IRequestHandler<CreateCommentCommand, Guid?>
{
    private readonly IApplicationDbContext _context;

    public CreateCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> Handle(
        CreateCommentCommand request,
        CancellationToken cancellationToken)
    {
        // Проверяем, существует ли задача
        var taskExists = await _context.Tasks
            .AnyAsync(
                t => t.Id == request.TaskId,
                cancellationToken);

        // Если задача не найдена - null
        if (!taskExists)
        {
            return null;
        }

        // Проверяем, существует ли пользователь
        var userExists = await _context.Users
            .AnyAsync(
                u => u.Id == request.UserId,
                cancellationToken);

        // Если пользователь не найден - null
        if (!userExists)
        {
            return null;
        }

        // Создаем новый комментарий
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            TaskId = request.TaskId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        // Сохраняем комментарий в бд
        await _context.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}