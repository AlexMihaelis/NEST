using FluentValidation;

namespace NEST.Application.TODO.Columns;

// Validator проверяет данные команды до выполнения Handler
public class CreateColumnCommandValidator : AbstractValidator<CreateColumnCommand>
{
    public  CreateColumnCommandValidator()
    {
        // Название колонки обязательно и не должно превышать 100 символов
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        // Id доски обязателен
        RuleFor(c => c.BoardId)
            .NotEmpty();
    }
}