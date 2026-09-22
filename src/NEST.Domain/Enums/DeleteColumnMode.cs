namespace NEST.Domain.Enums;

// Определяет, что делать с задачами при удалении колонки
public enum DeleteColumnMode
{
    // Удалить колонку вместе с задачами
    DeleteTasks,
    
    // Перенести задачи в другую колонку
    MoveTasks
}