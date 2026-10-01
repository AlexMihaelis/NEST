namespace NEST.API.Controllers.Requests.Tasks;

// Данные, которые клиент передает для перемещения задачи
public class MoveTaskRequest
{
    public Guid TargetColumnId { get; set; }
    public int TargetPosition { get; set; }
}