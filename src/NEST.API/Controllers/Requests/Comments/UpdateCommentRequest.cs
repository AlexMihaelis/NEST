namespace NEST.API.Controllers.Requests.Comments;

// Данные, которые клиент передает для обновления комментария
public class UpdateCommentRequest
{
    public required string Content { get; set; }
}