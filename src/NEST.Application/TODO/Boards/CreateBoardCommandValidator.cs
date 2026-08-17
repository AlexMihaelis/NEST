using FluentValidation;

namespace NEST.Application.TODO.Boards;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");
        RuleFor(x =>  x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}