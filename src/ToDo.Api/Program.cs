using ToDo.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]!);
}, ServiceLifetime.Singleton);

var app = builder.Build();

app.MapControllers();

var context = app.Services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

app.Run();

