using FluentValidation;

namespace NEST.Application.TODO.Tasks;

// Validator проверяет данные команды до выполнения Handler
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        // Название обязательно и не должно превышать 100 символов
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);

        // Описание обязательно и не должно превышать 1000 символов
        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(1000);

        // Id колонки обязателен
        RuleFor(c => c.ColumnId)
            .NotEmpty();
        
        // Приоритет должен соответствовать одному из значений enum-а TaskPriority
        RuleFor(c => c.Priority)
            .IsInEnum();
    }
}