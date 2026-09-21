using FluentValidation;
using FluentValidation.Validators;

namespace NEST.Application.Users;

// Validator проверяет данные команды до выполнения Handler
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{

    public CreateUserCommandValidator()
    {
        // UserName обязателен и не должен превышать 50 символов
        RuleFor(u => u.UserName)
            .NotEmpty()
            .MaximumLength(50);
        
        // Email обязателен, должен иметь корректный формат и не должен превышать 255 символов
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
    }
    
}