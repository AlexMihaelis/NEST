using NEST.Domain.Enums;

namespace NEST.Application.TODO.Tasks;

// Данные, которые клиент передает для создания задачи
public class CreateTaskRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public Guid? TaskContextId { get; set; }
    public Guid ColumnId { get; set; }
}