using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minio;
using NEST.API.Common.Middleware;
using NEST.Application;
using NEST.Application.Common.Behaviors;
using NEST.Application.Common.Interfaces;
using NEST.Infrastructure.Data;
using NEST.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

// Настраиваем CORS для Angular-клиента.
// Разрешаем frontend обращаться к API с локального адреса разработки.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Настройка Dependency Injection (DI - внедрение зависимостей):
// Регистрируем DbContext и подключаем PostgreSQL
builder.Services.AddDbContext<NestDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем интерфейс, через который Application работает с бд, и связываем его с конкретным NestDbContext
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<NestDbContext>());

// Регистрируем MediatR и говорим ему искать Commands, Queries и Handlers в сборке NEST.Application
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Регистрируем все FluentValidation-валидаторы из NEST.Application
builder.Services.AddValidatorsFromAssembly(
    typeof(AssemblyReference).Assembly);

// Добавляем ValidationBehavior в Pipeline MediatR, чтобы запросы проходили валидацию до вызова Handler
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

// Регистрируем настройки MinIO из конфигурации приложения
builder.Services
    .AddOptions<MinioOptions>()
    .Bind(builder.Configuration.GetSection("Minio"))
    .ValidateOnStart();

// Регистрируем MinIO client и настраиваем подключение к локальному MinIO
builder.Services.AddMinio(client =>
    client
        .WithEndpoint(builder.Configuration["Minio:Endpoint"]!)
        .WithCredentials(
            builder.Configuration["Minio:AccessKey"]!,
            builder.Configuration["Minio:SecretKey"]!)
        .WithSSL(false));

// Регистрируем реализацию IFileStorage для работы с MinIO
builder.Services.AddScoped<IFileStorage, MinioFileStorage>();

// Регистрируем сервис инициализации bucket в MinIO
builder.Services.AddScoped<MinioBucketInitializer>();

// Регистрируем фоновый сервис для автоматического удаления
// Attachment, которые остаются без Task и Comment дольше 30 дней
builder.Services.AddHostedService<OrphanedAttachmentCleanupService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Позволяем передавать enum в JSON в виде строк
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

var app = builder.Build();

// Создаем необходимый bucket в MinIO при запуске приложения
using (var scope = app.Services.CreateScope())
{
    var bucketInitializer = scope.ServiceProvider
        .GetRequiredService<MinioBucketInitializer>();

    await bucketInitializer.InitializeAsync();
}

// Подключаем middleware для обработки ошибок валидации
// Он должен находиться перед контроллерами, чтобы перехватывать исключения, возникающие в следующих компонентах Pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Применяем CORS-политику для Angular-клиента.
app.UseCors("AngularClient");

app.MapControllers();

app.Run();