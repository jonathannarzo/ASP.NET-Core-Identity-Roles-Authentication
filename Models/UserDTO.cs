namespace api.Models;

using System.ComponentModel.DataAnnotations;

public class LoginUserDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [StringLength(15, ErrorMessage = "Your Password is limited to")]
    public string Password { get; set; }
}

public class UserDTO : LoginUserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<string> Roles { get; set; }

}

public class UpdateUserDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<string> Roles { get; set; }
}