using NEST.Domain.Enums;

namespace NEST.Application.TODO.Tasks;

// Данные, которые клиент передает для обновления задачи
public class UpdateTaskRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? TaskContextId { get; set; }
}