using ToDo.Api.Data;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Register AppDbContext and set its lifetime as a singleton to make sure that there will be only one instance of it.
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

