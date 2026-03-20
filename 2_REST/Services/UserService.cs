using _2_REST.Dtos;

namespace _2_REST.Services;


class UserService
{
    List<UserDto> users = new List<UserDto>
    {
        new() {Id = 1, Name = "Ivan", Email = "Ivanov", Age = 20},
        new() {Id = 2, Name = "Petr", Email = "Petrov"},
        new() {Id = 3, Name = "Sidor", Email = "Sidorov", Age = 40}
    };

    

}
