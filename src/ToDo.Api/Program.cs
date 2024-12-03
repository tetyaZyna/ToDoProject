using ToDo.Api.Data;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Register AppDbContext and set its lifetime as a singleton to make sure that there will be only one instance of it.
builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]!);
}, ServiceLifetime.Singleton);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });
});

builder.Services.AddScoped<IToDoService, ToDoService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();

app.MapControllers();

var context = app.Services.GetRequiredService<AppDbContext>();
await context.Database.MigrateAsync();

app.Run();

