using FluentValidation;

namespace NEST.Application.TODO.Comments;

// Validator проверяет данные команды до выполнения Handler
public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        // Id комментария обязателен
        RuleFor(c => c.CommentId)
            .NotEmpty();
        
        // Текст комментария обязателен и не должен превышать 2000 символов
        RuleFor(c => c.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}