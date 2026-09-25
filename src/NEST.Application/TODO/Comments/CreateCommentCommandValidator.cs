using FluentValidation;

namespace NEST.Application.TODO.Comments;

// Validator проверяет данные команды до выполнения Handler
public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        // текст комментария обязателен и не должен превышать 2000 символов
        RuleFor(c => c.Content)
            .NotEmpty()
            .MaximumLength(2000);
        
        // Id задачи обязателен
        RuleFor(c => c.TaskId)
            .NotEmpty();
        
        // Id пользователя обязателен
        RuleFor(c => c.UserId)
            .NotEmpty();
    }
}