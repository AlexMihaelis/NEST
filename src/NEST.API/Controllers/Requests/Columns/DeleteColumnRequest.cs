using NEST.Domain.Enums;

namespace NEST.API.Controllers.Requests.Columns;

// Данные, которые клиент передает для удаления колонки
public class DeleteColumnRequest
{
    public DeleteColumnMode Mode { get; set; }
    public Guid? TargetColumnId { get; set; }
}