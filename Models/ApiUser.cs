namespace api.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ApiUser : IdentityUser, IAuditable
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }

    public DateTimeOffset? DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
}