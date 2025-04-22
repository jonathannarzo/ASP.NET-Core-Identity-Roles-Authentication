namespace api.Models;

using System.ComponentModel.DataAnnotations;
using api.Validation;

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
    [StrictEmailValidation]
    public string Email { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public ICollection<string> Roles { get; set; }

}

public class UpdateUserDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public ICollection<string> Roles { get; set; }
}

public class UpdateProfileDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}

public class ChangePasswordDTO
{
    [Required]
    public string CurrentPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
}