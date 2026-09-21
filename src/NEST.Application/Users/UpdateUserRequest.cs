namespace NEST.Application.Users;

// Request содержит только данные, которые клиент может передать при обновлении пользователя
// Id здесь нет, потому что он передается отдельно в URL
public class UpdateUserRequest
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public DateOnly BirthDate { get; set; }
}