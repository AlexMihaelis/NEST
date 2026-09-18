using FluentValidation;

namespace NEST.Application.TODO.Boards;

public class UpdateBoardCommandValidator : AbstractValidator<UpdateBoardCommand>
{
    public UpdateBoardCommandValidator()
    {
        // Проверяем Id доски
        RuleFor(b => b.BoardId)
            .NotEmpty()
            .WithMessage("BoardId is required");
        
        // Првоеряем название доски
        RuleFor(b => b.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");
        
        // Описание необязательно, но ограничено - 1000 символов
        RuleFor(b => b.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");
    }
}