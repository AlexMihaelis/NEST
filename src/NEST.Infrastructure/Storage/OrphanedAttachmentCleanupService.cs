using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NEST.Application.Common.Interfaces;

namespace NEST.Infrastructure.Storage;

// Периодически удаляет Attachment, которые остаются без Task и Comment дольше установленного срока
public class OrphanedAttachmentCleanupService : BackgroundService
{
    // Attachment считается сиротой после 30 дней без Task и Comment
    private static readonly TimeSpan OrphanedLifetime = TimeSpan.FromDays(30);

    // Проверяем наличие старых Attachment один раз в сутки
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromDays(1);

    private readonly IServiceScopeFactory _scopeFactory;

    public OrphanedAttachmentCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Проверяем сразу после запуска приложения, а затем повторяем проверку через установленный интервал
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupAsync(stoppingToken);

            await Task.Delay(
                CleanupInterval,
                stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        var fileStorage = scope.ServiceProvider
            .GetRequiredService<IFileStorage>();

        var expirationDate = DateTime.UtcNow - OrphanedLifetime;

        var attachments = await context.Attachments
            .Where(a =>
                a.TaskId == null &&
                a.CommentId == null &&
                a.OrphanedAt != null &&
                a.OrphanedAt <= expirationDate)
            .ToListAsync(cancellationToken);

        foreach (var attachment in attachments)
        {
            // Сначала удаляем файл из MinIO
            await fileStorage.DeleteAsync(
                attachment.StorageKey,
                cancellationToken);

            // После успешного удаления файла удаляем его запись из бд
            context.Attachments.Remove(attachment);
        }

        if (attachments.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}