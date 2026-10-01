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
        var comment = await _context.Comments
            .FirstOrDefaultAsync(
                c => c.Id == request.CommentId,
                cancellationToken);

        if (comment is null)
        {
            return false;
        }

        // Находим Attachments, которые связаны только с этим Comment
        // Если Attachment также связан с Task, после удаления Comment он все равно останется доступен через Task.
        var attachmentsToOrphan = await _context.Attachments
            .Where(a =>
                a.CommentId == comment.Id &&
                a.TaskId == null)
            .ToListAsync(cancellationToken);

        var orphanedAt = DateTime.UtcNow;

        foreach (var attachment in attachmentsToOrphan)
        {
            attachment.OrphanedAt = orphanedAt;
        }

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}