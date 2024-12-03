using ToDo.Api.Data;
using ToDo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Services;

public class ToDoService : IToDoService
{
    private readonly AppDbContext _dbContext;

    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ToDoModel?> FindByIdAsync(int id)
    {
        return await _dbContext.ToDos.FindAsync(id);
    }

    public async Task<IEnumerable<ToDoModel>> GetAllAsync()
    {
        return await _dbContext.ToDos.ToListAsync();
    }

    public async Task<IEnumerable<ToDoModel>?> FindByExpireDateAsync(DateTime from, DateTime to)
    {
        //error indication
        if (from > to) 
        {
            return null;
        }
        return await _dbContext.ToDos.Where(todo => todo.ExpiryDate >= from && todo.ExpiryDate <= to).ToListAsync();
    }

    public async Task<ToDoModel?> CreateAsync(ToDoModel toDo)
    { 
        //Validate field to make impossible to save an empty title   
        if (string.IsNullOrWhiteSpace(toDo.Title))
        {
            return null;
        }
        //Validate field to make impossible to save the date later than the time now
        if (toDo.ExpiryDate < DateTime.UtcNow)
        {
            return null;
        }
        //set the ID to zero to not interfere with the auto generation in the database
        toDo.Id = 0;
        toDo.CompletionPercentage = 0;

        _dbContext.ToDos.Add(toDo);
        await _dbContext.SaveChangesAsync();
        return toDo;
    }

    public async Task<bool?> UpdateByIdAsync(int id, ToDoModel toDo)
    {  
        var existingToDo = await _dbContext.ToDos.FindAsync(id);

        if (existingToDo is null) 
        {
            return null;
        }

        bool isUpdated = false;

        // Update title, description, or expiry date if they differ and are valid
        if (existingToDo.Title != toDo.Title && !string.IsNullOrWhiteSpace(toDo.Title))
        {
            existingToDo.Title = toDo.Title;
            isUpdated = true;
        }

        if (existingToDo.Description != toDo.Description && toDo.Description != null)
        {
            existingToDo.Description = toDo.Description;
            isUpdated = true;
        }

        if (existingToDo.ExpiryDate != toDo.ExpiryDate && toDo.ExpiryDate > DateTime.UtcNow)
        {
            existingToDo.ExpiryDate = toDo.ExpiryDate;
            isUpdated = true;
        }

        await _dbContext.SaveChangesAsync();
        return isUpdated;
    }

    public async Task<bool?> UpdateCompletionPercentageByIdAsync(int id, int completionPercentage)
    {  
        var existingToDo = await _dbContext.ToDos.FindAsync(id);

        if (existingToDo is null) 
        {
            return null;
        }

        bool isUpdated = false;

        // Update сompletion percentage if it differ and valid
        if (existingToDo.CompletionPercentage != completionPercentage && completionPercentage is >= 0 and <= 100)
        {
            existingToDo.CompletionPercentage = completionPercentage;
            isUpdated = true;
        }       

        await _dbContext.SaveChangesAsync();
        return isUpdated;
    }

    public async Task<bool?> DeleteAsync(int id)
    {  
        var existingToDo = await _dbContext.ToDos.FindAsync(id);

        if (existingToDo is null) 
        {
            return null;
        }
        
        _dbContext.Remove(existingToDo);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
