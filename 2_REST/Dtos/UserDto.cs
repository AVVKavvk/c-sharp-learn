using System.ComponentModel.DataAnnotations;

namespace _2_REST.Dtos;

public record class UserDto
{
    [Required][Range(0,100000)] public int Id {get; set;}
    [Required][MaxLength(100)] public string Name {get; set;} = string.Empty;

    [Required][MaxLength(100)] public string Email {get; set;} = string.Empty;

    public int? Age {get; set;}

}
