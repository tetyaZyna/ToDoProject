using ToDo.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ToDo.Api.Tests.Integration;

public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
        .WithUsername("postgres")
        .WithPassword("12345")
        .WithDatabase("ToDos")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(x =>
        {
            x.Remove(x.Single(a => typeof(DbContextOptions<AppDbContext>) == a.ServiceType));
            x.AddDbContext<AppDbContext>(a =>
            {
                a.UseNpgsql(_databaseContainer.GetConnectionString());
            }, ServiceLifetime.Singleton);
        });
    }

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _databaseContainer.StopAsync();
    }
}
