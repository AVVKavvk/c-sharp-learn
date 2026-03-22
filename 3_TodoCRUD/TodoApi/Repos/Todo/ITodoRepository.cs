using TodoApi.Models.Todo;

namespace TodoApi.Repos.Todo;

public interface ITodoRepository
{
    Task<IEnumerable<TodoModel>> GetAllAsync(int page = 1, int limit = 20);
    Task<TodoModel?> GetByIdAsync(int id);

    Task<TodoModel> CreateAsync(TodoModel todo);
    Task<TodoModel?> UpdateAsync(int id, TodoModel todo);
    Task<bool> DeleteAsync(int id);
}
