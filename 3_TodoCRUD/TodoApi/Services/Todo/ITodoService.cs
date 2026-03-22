using TodoApi.Dtos.Todo;
using TodoApi.Models.Todo;

namespace TodoApi.Services.Todo;

public interface ITodoService
{
    Task<IEnumerable<TodoModel>> GetAllAsync(int page = 1, int limit = 20);
    Task<TodoModel?> GetByIdAsync(int id);
    Task<TodoModel> CreateAsync(CreateTodoDto dto);
    Task<TodoModel?> UpdateAsync(int id, UpdateTodoDto dto);
    Task<bool> DeleteAsync(int id);
}
