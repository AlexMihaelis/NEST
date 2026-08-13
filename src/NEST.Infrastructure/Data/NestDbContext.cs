using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities;
using NEST.Domain.Entities.TODO;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Infrastructure.Data;

public class NestDbContext(DbContextOptions<NestDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskContext> TaskContexts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Column> Columns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Column>()
            .HasOne(c => c.Board)
            .WithMany(b => b.Columns)
            .HasForeignKey(c => c.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.Column)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.TaskContext)
            .WithMany(tc => tc.Tasks)
            .HasForeignKey(t => t.TaskContextId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Task)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Comment)
            .WithMany(c => c.Attachments)
            .HasForeignKey(a => a.CommentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Board>()
            .HasOne(b => b.User)
            .WithMany(u => u.Boards)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Comment>()
            .Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);
            
        modelBuilder.Entity<Board>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<Board>()
            .Property(b => b.Description)
            .HasMaxLength(1000);
        
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        modelBuilder.Entity<Column>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<Task>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        modelBuilder.Entity<Task>()
            .Property(t => t.Description)
            .HasMaxLength(5000);
        
        modelBuilder.Entity<TaskContext>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<Attachment>()
            .Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);
        
        modelBuilder.Entity<Attachment>()
            .Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<Attachment>()
            .Property(a => a.StorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Task>()
            .Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}