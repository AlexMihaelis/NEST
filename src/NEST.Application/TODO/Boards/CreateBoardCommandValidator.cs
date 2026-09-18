using FluentValidation;

namespace NEST.Application.TODO.Boards;

// Валидатор для проверки данных CreateBoardCommand перед выполнением Handler
public class CreateBoardCommandValidator
    : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        // Проверяем название доски
        RuleFor(b => b.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        // Проверяем описание доски
        // Описание может быть пустым, но не должно превышать 1000 символов
        RuleFor(b => b.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        // Проверяем, что передан Id пользователя
        RuleFor(b => b.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}