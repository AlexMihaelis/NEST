using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в DeleteCommentCommand
public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(
            c => c.Id == request.CommentId,
            cancellationToken);
        
        if (comment is null)
        {
            return false;
        }
        
        _context.Comments.Remove(comment);
        
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}