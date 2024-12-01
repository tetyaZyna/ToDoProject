using ToDo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ToDoModel> ToDos { get; set; } = null!;
}
