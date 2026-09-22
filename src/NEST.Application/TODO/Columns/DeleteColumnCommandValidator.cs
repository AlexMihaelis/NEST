using FluentValidation;
using NEST.Domain.Enums;

namespace NEST.Application.TODO.Columns;

// Validator проверяет данные команды до выполнения Handler
public class DeleteColumnCommandValidator : AbstractValidator<DeleteColumnCommand>
{
    public DeleteColumnCommandValidator()
    {
        // Id колонки обязателен
        RuleFor(c => c.ColumnId)
            .NotEmpty();

        // Если выбрано перемещение задач, должна быть указана целевая колонка
        RuleFor(c => c.TargetColumnId)
            .NotEmpty()
            .When(c => c.Mode == DeleteColumnMode.MoveTasks);
        
        // Нельзя переносить задачи в ту же самую колонку
        RuleFor(c => c.TargetColumnId)
            .NotEqual(c => c.ColumnId)
            .When(c => c.Mode == DeleteColumnMode.MoveTasks);
    }
}