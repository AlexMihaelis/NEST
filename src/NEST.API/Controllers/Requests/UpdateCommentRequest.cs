namespace NEST.API.Controllers.Requests;

// Данные, которые клиент передает для обновления комментария
public class UpdateCommentRequest
{
    public required string Content { get; set; }
}