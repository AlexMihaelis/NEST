using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application;
using NEST.Application.Common.Behaviors;
using NEST.Application.Common.Interfaces;
using NEST.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

// Настройка Dependency Injection (DI - внедрение зависимостей):
// Регистрируем DbContext и подключаем PostgreSQL
builder.Services.AddDbContext<NestDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем интерфейс, через который Application работает с бд, и связываем его с конкретным NestDbContext
builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<NestDbContext>());

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

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();