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
        var taskExists = await _context.Tasks
            .AnyAsync(t => t.Id == request.TaskId, cancellationToken);
        
        if (!taskExists)
        {
            return null;
        }
        
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (!userExists)
        {
            return null;
        }
        
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
        
        await _context.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}