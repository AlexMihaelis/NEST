using FluentValidation;

namespace NEST.Application.TODO.Tasks;

// Validator проверяет данные команды до выполнения Handler
public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public  UpdateTaskCommandValidator()
    {
        // Название обязательно и не должно превышать 100 символов
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        // Описание обязательно и не должно превышать 1000 символов
        RuleFor(t => t.Description)
            .NotEmpty()
            .MaximumLength(1000);
        
        // Id задачи обязателен
        RuleFor(t => t.TaskId)
            .NotEmpty();
        
        // Приоритет должен соответствовать одному из значений TaskPriority
        RuleFor(t => t.Priority)
            .IsInEnum();
    }
}