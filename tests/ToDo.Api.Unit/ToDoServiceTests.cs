using Microsoft.EntityFrameworkCore;
using ToDo.Api.Data;
using ToDo.Api.Models;
using ToDo.Api.Services;

namespace ToDo.Api.Unit;

public class ToDoServiceTests
{
    [Fact]
    public async Task FindByIdAsync_ReturnsSpecificToDo_WhenToDoExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.FindByIdAsync(toDo.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(toDo.Id, result.Id);
        Assert.Equal(toDo.Title, result.Title);
        Assert.Equal(toDo.Description, result.Description);
        Assert.Equal(toDo.CompletionPercentage, result.CompletionPercentage);
        Assert.Equal(toDo.ExpiryDate, result.ExpiryDate);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_WhenToDoIsNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.FindByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOneToDo_WhenOneToDoExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var resultTodo = result.First();
        Assert.NotNull(result);
        Assert.Equal(toDo.Id, resultTodo.Id);
        Assert.Equal(toDo.Title, resultTodo.Title);
        Assert.Equal(toDo.Description, resultTodo.Description);
        Assert.Equal(toDo.CompletionPercentage, resultTodo.CompletionPercentage);
        Assert.Equal(toDo.ExpiryDate, resultTodo.ExpiryDate);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindByExpireDateAsync_ReturnsToDoList_WhenDataIsValidAndToDoIsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        DateTime from = DateTime.UtcNow.AddDays(-10);
        DateTime to = DateTime.UtcNow.AddDays(10);

        // Act
        var result = await service.FindByExpireDateAsync(from, to);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var resultTodo = result.First();
        Assert.NotNull(result);
        Assert.Equal(toDo.Id, resultTodo.Id);
        Assert.Equal(toDo.Title, resultTodo.Title);
        Assert.Equal(toDo.Description, resultTodo.Description);
        Assert.Equal(toDo.CompletionPercentage, resultTodo.CompletionPercentage);
        Assert.Equal(toDo.ExpiryDate, resultTodo.ExpiryDate);
    }

    [Fact]
    public async Task FindByExpireDateAsync_ReturnsEmptyList_WhenDataIsValidAndToDoIsNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        DateTime yesterday = DateTime.UtcNow.AddDays(-1);
        DateTime tomorrow = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await service.FindByExpireDateAsync(yesterday, tomorrow);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindByExpireDateAsync_ReturnsNull_WhenDataIsNotValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        DateTime yesterday = DateTime.UtcNow.AddDays(-1);
        DateTime tomorrow = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await service.FindByExpireDateAsync(tomorrow, yesterday);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsToDo_WhenDataIsValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        var toDo = new ToDoModel
        {
            Title = "Test ToDo",
            Description = "Test Description",
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await service.CreateAsync(toDo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(toDo.Title, result.Title);
        Assert.Equal(toDo.Description, result.Description);
        Assert.Equal(0, result.CompletionPercentage);
        Assert.Equal(toDo.ExpiryDate, result.ExpiryDate);
    }

    [Fact]
    public async Task CreateAsync_ReturnsToDo_WhenTitleIsNotValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        var toDo = new ToDoModel
        {
            Description = "Test Description",
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = await service.CreateAsync(toDo);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsToDo_WhenDateIsNotValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        var toDo = new ToDoModel
        {
            Title = "Test ToDo",
            Description = "Test Description",
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = await service.CreateAsync(toDo);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateByIdAsync_ReturnsTrue_WhenToDoIsChanged()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        var toDoChanges = new ToDoModel
        {
            Title = "Test ToDo change",
            Description = "Test Description change",
            ExpiryDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await service.UpdateByIdAsync(toDo.Id ,toDoChanges);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateByIdAsync_ReturnsFalse_WhenToDoIsNotChangedOrDataIsNotValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        var toDoChanges = new ToDoModel
        {
            Title = toDo.Title,
            Description = toDo.Description,
            ExpiryDate = DateTime.UtcNow.AddDays(-100)
        };

        // Act
        var result = await service.UpdateByIdAsync(toDo.Id, toDoChanges);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateByIdAsync_ReturnsNull_WhenToDoIsNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        var toDoChanges = new ToDoModel
        {
            Title = "Test ToDo change",
            Description = "Test Description change",
            ExpiryDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await service.UpdateByIdAsync(10, toDoChanges);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCompletionPercentageByIdAsync_ReturnsTrue_WhenToDoIsChanged()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);    

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        int CompletionPercentageChange = 30;

        // Act
        var result = await service.UpdateCompletionPercentageByIdAsync(toDo.Id ,CompletionPercentageChange);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCompletionPercentageByIdAsync_ReturnsFalse_WhenToDoIsNotChanged()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        int CompletionPercentageChange = toDo.CompletionPercentage;

        // Act
        var result = await service.UpdateCompletionPercentageByIdAsync(toDo.Id ,CompletionPercentageChange);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCompletionPercentageByIdAsync_ReturnsFalse_WhenDataIsNotValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        int CompletionPercentageChange = -1;

        // Act
        var result = await service.UpdateCompletionPercentageByIdAsync(toDo.Id ,CompletionPercentageChange);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCompletionPercentageByIdAsync_ReturnsNull_WhenToDoIsNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        int CompletionPercentageChange = -1;

        // Act
        var result = await service.UpdateCompletionPercentageByIdAsync(10, CompletionPercentageChange);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenToDoIsExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var toDo = new ToDoModel
        {
            Id = 1,
            Title = "Test ToDo",
            Description = "Test Description",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow
        };

        dbContext.ToDos.Add(toDo);
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.DeleteAsync(toDo.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNull_WhenToDoIsNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new AppDbContext(options);

        var service = new ToDoService(dbContext);

        // Act
        var result = await service.DeleteAsync(10);

        // Assert
        Assert.Null(result);
    }
}