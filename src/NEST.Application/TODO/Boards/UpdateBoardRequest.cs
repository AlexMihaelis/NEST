namespace NEST.Application.TODO.Boards;

// Данные, которые клиент передает для обновления доски
public record UpdateBoardRequest(
    string Name,
    string? Description);