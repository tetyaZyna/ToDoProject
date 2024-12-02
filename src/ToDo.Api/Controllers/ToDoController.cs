using ToDo.Api.Services;
using Microsoft.AspNetCore.Mvc;
using ToDo.Api.Models;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class ToDoController : ControllerBase
{
    private readonly IToDoService _toDoService;

    public ToDoController(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    [HttpGet("")]
    public async Task<IResult> GetAll()
    {
        var todos = await _toDoService.GetAllAsync();
        return Results.Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetById(int id)
    {
        var todo = await _toDoService.FindByIdAsync(id);
        if (todo is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(todo);
    }

    [HttpGet("expiry")]
    public async Task<IResult> GetByExpiryDate(DateTime from, DateTime to)
    {
        if (from > to) 
        {
            return Results.BadRequest("The start date cannot be later than the end date.");
        }

        var todo = await _toDoService.FindByExpireDateAsync(from, to);
        
        return Results.Ok(todo);
    }

    [HttpPost("")]
    public async Task<IResult> CreateToDo(ToDoModel toDo)
    {
        if (string.IsNullOrWhiteSpace(toDo.Title))
        {
            return Results.BadRequest(new { Error = "Title is required" });
        }
        if (toDo.ExpiryDate < DateTime.UtcNow)
        {
            return Results.BadRequest(new { Error = "Expiry date cannot be in the past" });
        }
        toDo.Id = 0;
        toDo.CompletionPercentage = 0;

        var createdToDo = await _toDoService.CreateAsync(toDo);

        if (createdToDo is null) 
        {
            return Results.BadRequest("Failed to create ToDo. Please try again.");
        }
        
        return Results.Created($"/api/todos/{createdToDo.Id}", createdToDo);
    }

    [HttpPut("{id}")]
    public async Task<IResult> UpdateToDo(int id, ToDoModel toDo)
    {
        var isUpdated = _toDoService.UpdateByIdAsync(id, toDo);
        bool? result = await isUpdated;
        if (!result.HasValue)
        {
            return Results.NotFound();
        } 
        else if (result.HasValue && !result.Value)
        {
            return Results.BadRequest("No changes detected");
        }
        
        return Results.NoContent();
    }

    [HttpPatch("{id}/completion")]
    public async Task<IResult> UpdateToDoCompletionPercentage(int id, int completionPercentage)
    {
        var isUpdated = _toDoService.UpdateCompletionPercentageByIdAsync(id, completionPercentage);
        bool? result = await isUpdated;
        if (!result.HasValue)
        {
            return Results.NotFound();
        } 
        else if (result.HasValue && !result.Value)
        {
            return Results.BadRequest("No changes detected");
        }
        
        return Results.NoContent();
    }

    [HttpPatch("{id}/mark_done")]
    public async Task<IResult> MarkDoneToDo(int id)
    {
        int doneCompletionPercentages = 100;
        var isUpdated = _toDoService.UpdateCompletionPercentageByIdAsync(id, doneCompletionPercentages);
        bool? result = await isUpdated;
        if (!result.HasValue)
        {
            return Results.NotFound();
        }
        
        return Results.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteToDo(int id)
    {
        var isDeleted = _toDoService.DeleteAsync(id);
        bool? result = await isDeleted;
        if (!result.HasValue)
        {
            return Results.NotFound();
        } 
        
        return Results.NoContent();
    }

}
