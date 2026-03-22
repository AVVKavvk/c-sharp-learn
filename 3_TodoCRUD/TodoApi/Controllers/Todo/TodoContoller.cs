using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos.Todo;
using TodoApi.Services.Todo;

namespace TodoApi.Controllers.Todo;

/// <summary>Todo CRUD endpoints</summary>
[ApiController]
[Route("api/v1/todos")]
[Produces("application/json")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    /// <summary>Get all todos with pagination</summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="limit">Items per page (default: 20)</param>
    /// <response code="200">Returns paginated list of todos</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var todos = await _service.GetAllAsync(page, limit);
        return Ok(todos);
    }

    /// <summary>Get a single todo by ID</summary>
    /// <param name="id">Todo ID</param>
    /// <response code="200">Todo found</response>
    /// <response code="404">Todo not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var todo = await _service.GetByIdAsync(id);
        return todo is null ? NotFound(new { message = $"Todo {id} not found" }) : Ok(todo);
    }

    /// <summary>Create a new todo</summary>
    /// <param name="dto">Todo creation payload</param>
    /// <response code="201">Todo created successfully</response>
    /// <response code="400">Invalid request body</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
    {
        var todo = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    /// <summary>Update an existing todo</summary>
    /// <param name="id">Todo ID</param>
    /// <param name="dto">Todo update payload</param>
    /// <response code="200">Todo updated successfully</response>
    /// <response code="404">Todo not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
    {
        var todo = await _service.UpdateAsync(id, dto);
        return todo is null ? NotFound(new { message = $"Todo {id} not found" }) : Ok(todo);
    }

    /// <summary>Delete a todo by ID</summary>
    /// <param name="id">Todo ID</param>
    /// <response code="204">Todo deleted successfully</response>
    /// <response code="404">Todo not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _service.DeleteAsync(id);
        return todo ? NoContent() : NotFound(new { message = $"Todo {id} not found" });
    }
}
