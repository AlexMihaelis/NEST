namespace NEST.Application.TODO.Boards;

public record BoardDto(
    Guid Id,
    string Name,
    string? Description,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt);