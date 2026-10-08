namespace NEST.API.Controllers.Requests.Columns;

// Данные, которые клиент передает для перемещения колонки
public class MoveColumnRequest
{
    public int TargetPosition { get; set; }
}