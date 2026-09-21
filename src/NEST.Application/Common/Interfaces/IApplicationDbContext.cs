using Microsoft.EntityFrameworkCore;
using NEST.Domain.Entities;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User>  Users { get; }
    
    DbSet<Board> Boards { get; }
    DbSet<Column> Columns { get; }
    DbSet<Task> Tasks { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<TaskContext> TaskContexts { get; }

    // Сохраняем все изменения в бд
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}