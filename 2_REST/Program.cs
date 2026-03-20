using _2_REST.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();



const string GetUserROuteName = "GetUserById";
 List<UserDto> users = new List<UserDto>
    {
        new() {Id = 1, Name = "Ivan", Email = "Ivanov", Age = 20},
        new() {Id = 2, Name = "Petr", Email = "Petrov"},
        new() {Id = 3, Name = "Sidor", Email = "Sidorov", Age = 40}
    };


app.MapGet("/", () => "Avvk");

app.MapGet("/users", () =>
{
    
    return Results.Ok(users);
});

app.MapGet("/users/{id}", (int id) =>
{
    UserDto? user = users.Find(u=>u.Id==id);

    if (user is null)
    {
        return Results.NotFound("User not found with id"+id);
    }
    return Results.Ok(user);

}).WithName(GetUserROuteName);


app.MapPost("/users", (UserDto user) =>
{
    int id = user.Id;

    UserDto? oldUser = users.Find(u => u.Id == id);

    if (oldUser is not null)
    {
        return Results.BadRequest("User with id " + id + " already exist");
    }
    users.Add(user);
    return Results.CreatedAtRoute(GetUserROuteName, new { id = user.Id}, user);
});


app.MapPut("/users/{id}", (int id, UserDto user) =>
{

    UserDto? oldUser = users.Find(u => u.Id == id);

    if (oldUser is null)
    {
        return Results.NotFound("User not found with id" + id);
    }

    oldUser.Name = user.Name;
    oldUser.Email = user.Email;
    oldUser.Age = user.Age;

    return Results.Ok(oldUser);
});


app.MapDelete("/users/{id}", (int id) =>
{
    UserDto? user = users.Find(u => u.Id == id);

    if (user is null)
    {
        return Results.NotFound("User not found with id" + id);
    }
    users.Remove(user);
    return Results.Ok();
});


app.Run();
