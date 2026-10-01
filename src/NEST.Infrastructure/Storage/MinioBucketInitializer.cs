using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace NEST.Infrastructure.Storage;


// Проверяет наличие необходимого bucket в MinIO и создает его, если bucket еще не существует
// Bucket подготавливается при запуске приложения, а не перед каждой загрузкой файла
public class MinioBucketInitializer
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioBucketInitializer(IMinioClient minioClient, IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }
    
    // Проверяет наличие bucket и создает его принеобходимости
    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        var args = new BucketExistsArgs().WithBucket(_options.BucketName);
        
        var bucketExists = await _minioClient.BucketExistsAsync(args, cancellationToken);

        if (bucketExists)
        {
            return;
        }
        
        var createBucketArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
        
        await _minioClient.MakeBucketAsync(createBucketArgs, cancellationToken);
    }
}