namespace NEST.Infrastructure.Storage;

// MinioOptions хранит настройки подключения к MinIO

// Вместо того чтобы прописывать адрес MinIO, логин, пароль и название bucket прямо в коде MinioFileStorage,
// мы получаем их из конфигурации приложения

// Это позволяет менять настройки без изменения самого класса, например, отдельно для Development, Docker и Production
public class MinioOptions
{
    // Адрес MinIO
    
    // При запуске NEST напрямую с компьютера: http://127.0.0.1:9000
    
    // Если позже сам NEST будет запускаться внутри Docker Compose, здесь уже можно будет использовать: http://minio:9000
    public required string Endpoint { get; set; }

    // Логин для подключения к MinIO
    public required string AccessKey { get; set; }

    // Пароль для подключения к MinIO
    public required string SecretKey { get; set; }

    // Название bucket, в котором будут храниться файлы
    // P.S.: Bucket - это контейнер верхнего уровня для объектов в object storage.
    public required string BucketName { get; set; }

    // Используется ли HTTPS при подключении к MinIO
    
    // Сейчас у нас локальный MinIO работает через HTTP, поэтому значение будет false
    public bool UseSsl { get; set; }
}