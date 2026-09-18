using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Infrastructure.Data;

// DbContext - основной класс EF Core для работы с бд. Через него получаем доступ к таблицам и настраиваем связи между сущностями
public class NestDbContext(
    DbContextOptions<NestDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    // DbSet представляет таблицу в бд
    public DbSet<User> Users { get; set; }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskContext> TaskContexts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }

    // Настраиваем связи и ограничения для сущностей EF Core
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Board имеет много Columns.
        // При удалении Board связанные Columns удаляются автоматически
        modelBuilder.Entity<Column>()
            .HasOne(c => c.Board)
            .WithMany(b => b.Columns)
            .HasForeignKey(c => c.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Column имеет много Tasks
        // При удалении Column связанные Tasks удаляются автоматически
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Column)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskContext может содержать много Tasks
        // При удалении TaskContext связь у Task обнуляется, а сама Task остается
        modelBuilder.Entity<Task>()
            .HasOne(t => t.TaskContext)
            .WithMany(tc => tc.Tasks)
            .HasForeignKey(t => t.TaskContextId)
            .OnDelete(DeleteBehavior.SetNull);

        // Task имеет много Comments
        // При удалении Task связанные Comments удаляются автоматически
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Task может иметь много Attachments
        // При удалении Task Attachment остается, но связь с Task становится null
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        // User может иметь много Comments
        // При удалении User связанные Comments автоматически не удаляются
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment может иметь много Attachments
        // При удалении Comment Attachment остается, но связь с Comment становится null
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Comment)
            .WithMany(c => c.Attachments)
            .HasForeignKey(a => a.CommentId)
            .OnDelete(DeleteBehavior.SetNull);

        // User может загрузить много Attachments
        // При удалении User его Attachments автоматически не удаляются
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User может иметь много Boards
        // При удалении User связанные Boards автоматически не удаляются
        modelBuilder.Entity<Board>()
            .HasOne(b => b.User)
            .WithMany(u => u.Boards)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Content обязателен и может содержать максимум 2000 символов
        modelBuilder.Entity<Comment>()
            .Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);
            
        // Name у Board обязателен и ограничен 100 символами
        modelBuilder.Entity<Board>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        // Description у Board необязателен, максимум 1000 символов
        modelBuilder.Entity<Board>()
            .Property(b => b.Description)
            .HasMaxLength(1000);
        
        // Username обязателен и ограничен 50 символами
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        // Email обязателен и ограничен 255 символами
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        // Name у Column обязателен и ограничен 100 символами
        modelBuilder.Entity<Column>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        // Name у Task обязателен и ограничен 200 символами
        modelBuilder.Entity<Task>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        // Description у Task необязателен, максимум 5000 символов
        modelBuilder.Entity<Task>()
            .Property(t => t.Description)
            .HasMaxLength(5000);
        
        // Name у TaskContext обязателен и ограничен 100 символами
        modelBuilder.Entity<TaskContext>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        // Имя файла обязательно и ограничено 255 символами
        modelBuilder.Entity<Attachment>()
            .Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);
        
        // ContentType обязателен и ограничен 100 символами
        modelBuilder.Entity<Attachment>()
            .Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);
        
        // StorageKey обязателен и ограничен 500 символами
        modelBuilder.Entity<Attachment>()
            .Property(a => a.StorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        // Email должен быть уникальным для каждого User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        // Username должен быть уникальным
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Enum Priority сохраняется в бд как строка, а не как числовое значение
        modelBuilder.Entity<Task>()
            .Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}