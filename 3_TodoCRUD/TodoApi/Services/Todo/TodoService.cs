using TodoApi.Dtos.Todo;
using TodoApi.Logger;
using TodoApi.Models.Todo;
using TodoApi.Repos.Todo;

namespace TodoApi.Services.Todo;

public class TodoService : ITodoService
{
    private ITodoRepository _repo;

    public TodoService(ITodoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<TodoModel>> GetAllAsync(int page = 1, int limit = 20)
    {
        try
        {
            return await _repo.GetAllAsync(page, limit);
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<TodoModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _repo.GetByIdAsync(id);
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<TodoModel> CreateAsync(CreateTodoDto dto)
    {
        try
        {
            var todo = new TodoModel { Title = dto.Title, Description = dto.Description };
            return await _repo.CreateAsync(todo);
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<TodoModel?> UpdateAsync(int id, UpdateTodoDto dto)
    {
        try
        {
            var todo = new TodoModel
            {
                Title = dto.Title ?? string.Empty,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted ?? false,
            };
            return await _repo.UpdateAsync(id, todo);
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            return await _repo.DeleteAsync(id);
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }
}
