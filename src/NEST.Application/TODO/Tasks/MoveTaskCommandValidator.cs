using FluentValidation;

namespace NEST.Application.TODO.Tasks;

// Validator проверяет данные команды до выполнения Handler
public class MoveTaskCommandValidator : AbstractValidator<MoveTaskCommand>
{
    public MoveTaskCommandValidator()
    {
        // Id задачи обязателен
        RuleFor(c => c.TaskId)
            .NotEmpty();

        // Id целевой колонки обязателен
        RuleFor(c => c.TargetColumnId)
            .NotEmpty();

        // Позиция не может быть отрицательной
        RuleFor(c => c.TargetPosition)
            .GreaterThanOrEqualTo(0);
    }
}