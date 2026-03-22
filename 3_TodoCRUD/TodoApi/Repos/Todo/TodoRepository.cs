using Dapper;
using Npgsql;
using TodoApi.Logger;
using TodoApi.Models.Todo;

namespace TodoApi.Repos.Todo;

public class TodoRepository : ITodoRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public TodoRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<TodoModel>> GetAllAsync(int page = 1, int limit = 20)
    {
        try
        {
            var testExtras = new Dictionary<string, object> { ["page"] = page, ["limit"] = limit };
            foreach (var kv in testExtras)
                Console.WriteLine($"key={kv.Key} val={kv.Value} type={kv.Value?.GetType().Name}");

            AppLog.Info("Get all todos", extras: testExtras);
            await using var conn = await _dataSource.OpenConnectionAsync();

            if (conn is null)
            {
                AppLog.Error("PG connection is null");
                throw new Exception("PG connection is null");
            }
            if (page < 1)
            {
                page = 1;
            }
            if (limit > 100)
            {
                limit = 100;
            }

            int offset = (page - 1) * limit;

            const string sql =
                "SELECT * FROM todos ORDER BY created_at DESC LIMIT @Limit OFFSET @Offset";
            var result = await conn.QueryAsync<TodoModel>(
                sql,
                new { Limit = limit, Offset = offset }
            );
            return result;
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
            AppLog.Info("Get todo by id", extras: new() { ["id"] = id });

            await using var conn = await _dataSource.OpenConnectionAsync();

            if (conn is null)
            {
                AppLog.Error("PG connection is null");
                throw new Exception("PG connection is null");
            }
            const string sql = "SELECT * FROM todos WHERE id = @Id";
            return await conn.QueryFirstOrDefaultAsync<TodoModel>(sql, new { Id = id });
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<TodoModel> CreateAsync(TodoModel todo)
    {
        try
        {
            AppLog.Info("Create todo", extras: new() { ["todo"] = todo });

            await using var conn = await _dataSource.OpenConnectionAsync();

            if (conn is null)
            {
                AppLog.Error("PG connection is null");
                throw new Exception("PG connection is null");
            }

            const string sql = """
                INSERT INTO todos (title, description, is_completed, created_at, updated_at)
                VALUES (@Title, @Description,false, NOW(), NOW())
                RETURNING *
                """;

            return await conn.QuerySingleAsync<TodoModel>(
                sql,
                new { Title = todo.Title, Description = todo.Description }
            );
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }

    public async Task<TodoModel?> UpdateAsync(int id, TodoModel todo)
    {
        try
        {
            AppLog.Info("Update todo", extras: new() { ["id"] = id });
            await using var conn = await _dataSource.OpenConnectionAsync();

            if (conn is null)
            {
                AppLog.Error("PG connection is null");
                throw new Exception("PG connection is null");
            }

            const string sql = """
                UPDATE todos
                SET
                   title = COALESCE(@Title, title),
                   description = COALESCE(@Description, description),
                   is_completed = COALESCE(@IsCompleted, is_completed),
                   updated_at   = NOW()
                WHERE id = @Id
                RETURNING *
                """;

            return await conn.QueryFirstOrDefaultAsync<TodoModel>(
                sql,
                new
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                }
            );
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
            AppLog.Info("Delete todo", extras: new() { ["id"] = id });

            await using var conn = await _dataSource.OpenConnectionAsync();

            if (conn is null)
            {
                AppLog.Error("PG connection is null");
                throw new Exception("PG connection is null");
            }

            const string sql = "DELETE FROM todos WHERE id = @Id";
            var rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
        catch (Exception e)
        {
            AppLog.Error(e.Message);
            throw;
        }
    }
}
