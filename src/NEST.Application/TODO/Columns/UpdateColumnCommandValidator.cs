using FluentValidation;
using MediatR;

namespace NEST.Application.TODO.Columns;

// Validator проверяет данные команды до выполнения Handler
public class UpdateColumnCommandValidator : AbstractValidator<UpdateColumnCommand>
{
    public UpdateColumnCommandValidator()
    {
        // Id колонки обязателен
        RuleFor(c => c.ColumnId)
            .NotEmpty();
        
        // Название колонки обязательно и не должно превышать 100 символов
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        // Позиция не может быть отрицательной
        RuleFor(c => c.Position)
            .GreaterThanOrEqualTo(0);
    }
}