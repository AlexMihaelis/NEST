using FluentValidation;

namespace NEST.Application.TODO.Comments;

// Validator проверяет данные команды до выполнения Handler
public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        // Id комментария обязателен
        RuleFor(c => c.CommentId)
            .NotEmpty();
    }
}