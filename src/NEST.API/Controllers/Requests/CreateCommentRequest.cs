namespace NEST.API.Controllers.Requests;

// Данные, которые клиент передает для создания комментария
public class CreateCommentRequest
{
    public required string Content { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
}