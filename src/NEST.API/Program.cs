using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Infrastructure.Data;
using NEST.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NestDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext> (
    provider => provider.GetService<NestDbContext>());

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();