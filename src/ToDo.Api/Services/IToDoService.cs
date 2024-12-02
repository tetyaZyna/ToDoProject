using ToDo.Api.Models;

namespace ToDo.Api.Services;

public interface IToDoService
{
    Task<ToDoModel?> FindByIdAsync(int id);

    Task<IEnumerable<ToDoModel>> GetAllAsync();

    Task<IEnumerable<ToDoModel>?> FindByExpireDateAsync(DateTime from, DateTime to);

    Task<ToDoModel?> CreateAsync(ToDoModel todo);

    Task<bool?> UpdateByIdAsync(int id, ToDoModel todo);

    Task<bool?> UpdateCompletionPercentageByIdAsync(int id, int completionPercentage);

    Task<bool?> DeleteAsync(int id);
}
