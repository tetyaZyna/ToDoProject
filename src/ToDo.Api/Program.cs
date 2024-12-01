using ToDo.Api.Data;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]!);
}, ServiceLifetime.Singleton);
builder.Services.AddScoped<IToDoService, ToDoService>();

var app = builder.Build();

app.MapControllers();

var context = app.Services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

app.Run();

