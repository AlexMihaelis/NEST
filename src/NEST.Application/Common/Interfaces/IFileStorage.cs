namespace NEST.Application.Common.Interfaces;

// IFileStorage описывает операции с файловым хранилищем

// Application знает только об этом интерфейсе и не знает, какое конкретно хранилище используется внутри
// Например, реализацией может быть MinIO, S3 или другое хранилище
public interface IFileStorage
{
    // Загружает файл в хранилище по указанному ключу
    
    // Stream содержит содержимое файла
    // StorageKey определяет, под каким ключом файл будет сохранен
    // ContentType - MIME-тип файла (например, image/png или application/pdf)
    Task UploadAsync(
        Stream stream,
        string storageKey,
        string contentType,
        CancellationToken cancellationToken = default);

    // Получает файл из хранилища по его ключу
    
    // Возвращает поток с содержимым файла
    // Вызывающий код отвечает за освобождение полученного Stream
    Task<Stream> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    // Удаляет файл из хранилища по его ключу
    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    // Проверяет, существует ли файл с указанным ключом в хранилище
    Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}