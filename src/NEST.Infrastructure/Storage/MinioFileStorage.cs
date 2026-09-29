using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using NEST.Application.Common.Interfaces;

namespace NEST.Infrastructure.Storage;

// Реализация IFileStorage для хранения файлов в MinIO

// Application работает только с IFileStorage и не знает, что внутри используется именно MinIO
// Благодаря этому MinIO можно заменить другим хранилищем, не изменяя Application-слой
public class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioFileStorage(
        IMinioClient minioClient,
        IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }

    // Загружает файл в MinIO
    
    // stream содержит содержимое файла
    // storageKey - ключ, под которым объект будет сохранен в bucket
    // contentType - MIME-тип файла, например image/png или application/pdf
    public async Task UploadAsync(
        Stream stream,
        string storageKey,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var args = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(
            args,
            cancellationToken);
    }

    // Загружает файл из MinIO и возвращает его содержимое в виде Stream
    
    // Вызывающий код отвечает за освобождение полученного Stream
    public async Task<Stream> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(
            args,
            cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    // Удаляет объект из MinIO по его ключу
    public async Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey);

        await _minioClient.RemoveObjectAsync(
            args,
            cancellationToken);
    }

    // Проверяет существование объекта в MinIO
    public async Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var args = new StatObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageKey);

        try
        {
            await _minioClient.StatObjectAsync(
                args,
                cancellationToken);

            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }
}