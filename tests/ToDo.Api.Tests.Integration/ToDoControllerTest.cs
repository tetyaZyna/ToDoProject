using ToDo.Api.Data;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Api.Models;
using System.Net.Http.Json;

namespace ToDo.Api.Tests.Integration;

public class ToDoControllerTest : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _client;
    private readonly AppDbContext _dbContext;

    public ToDoControllerTest(CustomerApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _dbContext = apiFactory.Services.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoItemsInDatabase()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("api/todos");

        // Assert
        response.EnsureSuccessStatusCode(); 

        var todoList = await response.Content.ReadFromJsonAsync<List<ToDoModel>>(); 
        Assert.NotNull(todoList);  
        Assert.Empty(todoList);
        
    }

    [Fact]
    public async Task GetAll_ReturnsOneToDo_WhenOneToDoExists()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        var todo = new ToDoModel
        {
            Id = 10,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 10,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("api/todos");

        // Assert
        response.EnsureSuccessStatusCode(); 

        var todoList = await response.Content.ReadFromJsonAsync<List<ToDoModel>>(); 
        Assert.NotNull(todoList);
        Assert.Single(todoList);
        var responseTodo = todoList.First();
        Assert.Equal(todo.Id, responseTodo.Id);
        Assert.Equal(todo.Title, responseTodo!.Title); 
        Assert.Equal(todo.Description, responseTodo.Description); 
        Assert.Equal(10, responseTodo.CompletionPercentage);
        Assert.Equal(todo.ExpiryDate, responseTodo.ExpiryDate); 
        
    }

    [Fact]
    public async Task GetById_ReturnsSpecificToDo_WhenToDoExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        var todo = new ToDoModel
        {
            Id = 10,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 10,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"api/todos/{todo.Id}");

        // Assert
        response.EnsureSuccessStatusCode(); 

        var responseTodo = await response.Content.ReadFromJsonAsync<ToDoModel>(); 
        Assert.NotNull(responseTodo);
        Assert.Equal(todo.Id, responseTodo.Id);
        Assert.Equal(todo.Title, responseTodo!.Title); 
        Assert.Equal(todo.Description, responseTodo.Description); 
        Assert.Equal(10, responseTodo.CompletionPercentage);
        Assert.Equal(todo.ExpiryDate, responseTodo.ExpiryDate); 
        
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenToDoNotExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("api/todos/10");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);        
    }

    [Fact]
    public async Task GetByExpiryDate_ReturnsSpecificToDosList_WhenToDoExists()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        var todo = new ToDoModel
        {
            Id = 10,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 10,
            ExpiryDate = DateTime.UtcNow
        };

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        DateTime utcYesterday = DateTime.UtcNow.AddDays(-1);
        DateTime utcTomorrow = DateTime.UtcNow.AddDays(1);
        string formattedUtcYesterday = utcYesterday.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        string formattedUtcTomorrow = utcTomorrow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        // Act
        var response = await _client.GetAsync($"api/todos/expiry?from={formattedUtcYesterday}&to={formattedUtcTomorrow}");

        // Assert
        response.EnsureSuccessStatusCode(); 

        var todoList = await response.Content.ReadFromJsonAsync<List<ToDoModel>>(); 
        Assert.NotNull(todoList);
        Assert.Single(todoList);
        var responseTodo = todoList.First();
        Assert.Equal(todo.Id, responseTodo.Id);
        Assert.Equal(todo.Title, responseTodo!.Title); 
        Assert.Equal(todo.Description, responseTodo.Description); 
        Assert.Equal(10, responseTodo.CompletionPercentage);
        Assert.Equal(todo.ExpiryDate, responseTodo.ExpiryDate); 
    }

    [Fact]
    public async Task GetByExpiryDate_ReturnsEmptyToDosList_WhenToDoNotExists()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        DateTime utcNow = DateTime.UtcNow;
        DateTime utcTomorrow = DateTime.UtcNow.AddDays(1);
        string formattedUtcNow = utcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        string formattedUtcTomorrow = utcTomorrow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        // Act
        var response = await _client.GetAsync($"api/todos/expiry?from={formattedUtcNow}&to={formattedUtcTomorrow}");

        // Assert
        response.EnsureSuccessStatusCode(); 

        var todoList = await response.Content.ReadFromJsonAsync<List<ToDoModel>>(); 
        Assert.NotNull(todoList);
        Assert.Empty(todoList);
    }

    [Fact]
    public async Task GetByExpiryDate_ReturnsBadRequest_WhenDateRangeIsNotValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"api/todos/expiry?from={DateTime.UtcNow.AddDays(2)}&to={DateTime.UtcNow}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);        
    }
        

    [Fact]
    public async Task CreateToDo_ReturnsCreatedToDo_WhenDataIsValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 10,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/todos", todo);

        // Assert
        response.EnsureSuccessStatusCode(); 

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);        

        var responseTodo = await response.Content.ReadFromJsonAsync<ToDoModel>(); 

        var location = response.Headers.Location!.ToString();   
        var expectedLocation = $"/api/todos/{responseTodo!.Id}";

        Assert.Equal(expectedLocation, location);

        Assert.NotNull(responseTodo); 
        Assert.Equal(todo.Title, responseTodo!.Title); 
        Assert.Equal(todo.Description, responseTodo.Description); 
        Assert.Equal(0, responseTodo.CompletionPercentage); 
        Assert.Equal(todo.ExpiryDate, responseTodo.ExpiryDate); 

    }

    [Fact]
    public async Task CreateToDo_ReturnsBadRequest_WhenDataIsNotValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Description = "Todo Description",
            CompletionPercentage = 10,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/todos", todo);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDo_ReturnsNoContent_WhenDataIsValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var existindTodo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        var updatedTodo = new ToDoModel
        {
            Title = "ToDo1 update",
            Description = "Todo Description update",
            CompletionPercentage = 50,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _dbContext.ToDos.Add(existindTodo);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PutAsJsonAsync($"api/todos/{existindTodo.Id}", updatedTodo);

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDo_ReturnsBadReques_WhenDataIsNotValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var existindTodo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        var updatedTodo = new ToDoModel
        {
            Title = existindTodo.Title,
            Description = existindTodo.Description,
            CompletionPercentage = existindTodo.CompletionPercentage,
            ExpiryDate = existindTodo.ExpiryDate
        };

        _dbContext.ToDos.Add(existindTodo);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PutAsJsonAsync($"api/todos/{existindTodo.Id}", updatedTodo);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDo_ReturnsNotFound_WhenToDoIsNotExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        var updatedTodo = new ToDoModel
        {
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"api/todos/10", updatedTodo);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDoCompletionPercentage_ReturnsNoContent_WhenDataIsValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        int updatedCompletionPercentage = 34;

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/todos/{todo.Id}/completion?completionPercentage={updatedCompletionPercentage}");
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDoCompletionPercentage_ReturnsBadRequest_WhenDataIsNotValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        int updatedCompletionPercentage = -34;

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/todos/{todo.Id}/completion?completionPercentage={updatedCompletionPercentage}");
        var response = await _client.SendAsync(requestMessage);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);        
    }

    [Fact]
    public async Task UpdateToDoCompletionPercentage_ReturnsNotFound_WhenToDoIsNotExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        int updatedCompletionPercentage = -34;

        // Act
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/todos/1/completion?completionPercentage={updatedCompletionPercentage}");
        var response = await _client.SendAsync(requestMessage);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);        
    }

    [Fact]
    public async Task MarkDoneToDo_ReturnsNoContent_WhenDataIsValid()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/todos/{todo.Id}/mark_done");
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);        
    }

    [Fact]
    public async Task MarkDoneToDo_ReturnsNotFount_WhenToDoIsNotExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        // Act
        var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/todos/10/mark_done");
        var response = await _client.SendAsync(requestMessage);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);        
    }

    [Fact]
    public async Task DeleteToDo_ReturnsNoContent_WhenDataIsValid()
    {
        // Arrange
         _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();
        var todo = new ToDoModel
        {
            Id = 1,
            Title = "ToDo1",
            Description = "Todo Description",
            CompletionPercentage = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _dbContext.ToDos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"api/todos/{todo.Id}");

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);        
    }

    [Fact]
    public async Task DeleteToDo_ReturnsNotFount_WhenToDoIsNotExist()
    {
        // Arrange
        _dbContext.ToDos.RemoveRange(_dbContext.ToDos);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"api/todos/10");


        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);        
    }

}